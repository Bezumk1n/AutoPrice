using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AutoPrice
{
    public class DoPriceList
    {
        private string destinationPath;
        public DoPriceList(string pricelistPath, string exceptionPath, string destinationPath, string addInfoPath, bool? fullPrice)
        {
            this.destinationPath        = destinationPath;
            string[] fileText           = WorkWithFile.OpenFile(pricelistPath);
            string[] exceptions         = WorkWithFile.OpenFile(exceptionPath);
            string[] addInfo            = WorkWithFile.OpenFile(addInfoPath);
            List <PriceModel> priceList = new List<PriceModel>();
            CultureInfo culture         = CultureInfo.CreateSpecificCulture("en-US");

            string[,] price;
            string[,] additionalInfo;

            // Нужно подсчитать количество знаков табуляции. Так мы поймем сколько будет столбцов у будущих массивов "additionalInfo" и "price"
            int rows = addInfo.GetUpperBound(0);
            int columns = 1;
            string str = addInfo[0];
            string tab = "\t";
            int index = 0;

            while ((index = str.IndexOf(tab, index)) != -1)
            {
                columns++;
                index = index + tab.Length;
            }
            additionalInfo = new string[rows, columns + 5]; // добавляем 5 столбцов для категорий

            // Заполняем массив данными
            for (int i = 0; i < rows; i++)
            {
                string[] temp = addInfo[i].Split('\t');
                for (int j = 0; j < columns; j++)
                {
                    additionalInfo[i, j] = temp[j];
                }

                if (additionalInfo[i, 5].Contains(';'))
                {
                    additionalInfo[i, 5] = additionalInfo[i, 5].Replace(";Успей купить", "");
                    additionalInfo[i, 5] = additionalInfo[i, 5].Replace(";SALE (Распродажа)", "");
                    additionalInfo[i, 5] = additionalInfo[i, 5].Substring(additionalInfo[i, 5].LastIndexOf(";") + 1);
                }
                
                // Так как у нас категория записана в 1 строчку, делим ее и добавляем в наши дополнительные столбцы (их может быть максимум 5)
                string[] catalogGroups = additionalInfo[i, 5].Split('/');

                for (int k = 0; k < catalogGroups.Length; k++)
                {
                    additionalInfo[i, 6 + k] = catalogGroups[k];
                }
            }
            //==================================================================================================

            // Проделываем все тоже самое с массивом "price"
            rows = fileText.GetUpperBound(0);
            columns = 0;
            str  = fileText[0];
            tab  = "\t";
            index   = 0;

            while ((index = str.IndexOf(tab, index)) != -1)
            {
                columns++;
                index = index + tab.Length;
            }
            price = new string[rows, columns + 9];

            // Заполняем массив данными
            for (int i = 0; i < rows; i++)
            {
                string[] temp = fileText[i].Split('\t');
                for (int j = 0; j < columns; j++)
                {
                    price[i, j] = temp[j];
                }
            }
            //==================================================================================================

            // Проверяем на нулевые цены ("0.00") и исключаем их, если таковые находятся
            for (int i = 0; i < rows; i++)
            {
                if (price[i, 5] == "0.00")
                {
                    price[i, 0] = "0";
                }
            }
            //==================================================================================================

            // Блок проверки наименований на наличие. 
            // Наименования с нулевым количеством на складах (учитываются склад Северянин, Пушкарев и магазин) не будут попадать в прайс-лист
            string zero = "0.00";
            price[0, 0] = "0";

            if (fullPrice == false)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (price[i, 7] == zero && price[i, 9] == zero && price[i, 11] == zero && price[i, 3] != "OUP ELT OL")
                    {
                        price[i, 0] = "0";
                    }
                }
            }
            // Это условие срабатывает если fullPrice = true
            else
            {
                string op = "OP!";
                string na = "NA!";
                for (int i = 0; i < rows; i++)
                {
                    if (price[i, 2].EndsWith(op) || price[i, 2].EndsWith(na) && price[i, 7] == zero && price[i, 9] == zero && price[i, 11] == zero)
                    {
                        price[i, 0] = "0";
                    }
                }
            }
            //==================================================================================================

            // Блок проверки наименований на "агентское вознаграждение". 
            // Агентское соглашение не будет попадать в прайс-лист
            string agent = "агентское вознаграждение";

            for (int i = 0; i < rows; i++)
            {
                if (price[i, 14] != null && price[i, 14].ToLower().StartsWith(agent))
                {
                    price[i, 0] = "0";
                }
            }
            //==================================================================================================

            // Блок проверки групп товаров. 
            // Если группа товаров равна группе из списка исключений, то такое наименование не будет попадать в прайс
            if (fullPrice == false)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < exceptions.Length; j++)
                    {
                        if (price[i, 3] == exceptions[j])
                        {
                            price[i, 0] = "0";
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < exceptions.Length; j++)
                    {
                        if (price[i, 3] == exceptions[j])
                        {
                            price[i, 0] = "0";
                        }
                    }
                    if (price[i, 3] == "RELOD Ltd. (RUR)" || price[i, 3] == "RELOD LTD." || price[i, 3] == "SELT" && price[i, 7] == "0.00" && price[i, 9] == "0.00" && price[i, 11] == "0.00")
                    {
                        price[i, 0] = "0";
                    }
                }
            }
            //==================================================================================================

            // В этом блоке проверяем количества по складам
            for (int i = 1; i < rows; i++)
            {
                double warehouseQTY = double.Parse(price[i, 7], culture) + double.Parse(price[i, 11], culture);
                double storeQTY     = double.Parse(price[i, 9], culture);

                if (warehouseQTY > 10)
                {
                    price[i, 7] = "Более 10 шт";
                }
                else if (warehouseQTY == 1)
                {
                    price[i, 7] = "Мало";
                }
                else
                {
                    price[i, 7] = warehouseQTY.ToString();
                }

                if (storeQTY > 10)
                {
                    price[i, 9] = "Более 10 шт";
                }
                else if (storeQTY == 1)
                {
                    price[i, 9] = "Мало";
                }
                else
                {
                    price[i, 9] = storeQTY.ToString();
                }
            }
            //==================================================================================================

            for (int i = 0; i < rows; i++)
            {
                if (price[i, 0] != "0")
                {
                    for (int j = 0; j < additionalInfo.GetUpperBound(0); j++)
                    {
                        if (price[i, 1] == additionalInfo[j, 0])
                        {
                            price[i, 15] = additionalInfo[j, 1];
                            price[i, 16] = additionalInfo[j, 2];
                            price[i, 17] = additionalInfo[j, 3];
                            price[i, 18] = additionalInfo[j, 4];
                            price[i, 19] = additionalInfo[j, 6];
                            price[i, 20] = additionalInfo[j, 7];
                            price[i, 21] = additionalInfo[j, 8];
                            price[i, 22] = additionalInfo[j, 9];
                            price[i, 23] = additionalInfo[j, 10];
                        }
                    }
                }
                Console.WriteLine($"Обработана {i + 1} позиция из {rows}");
            }

            // Переносим данные из массива в итоговый прайс
            for (int i = 0; i < rows; i++)
            {
                if (price[i, 0] != "0")
                {
                    priceList.Add(new PriceModel
                    {
                        ISBN            = price[i, 1],                          // присваиваем ISBN
                        Title           = price[i, 14],                         // присваиваем Наименование
                        Price           = double.Parse(price[i, 6], culture),   // присваиваем Цену
                        VAT             = double.Parse(price[i, 4], culture),   // присваиваем НДС
                        Group           = price[i, 3],                          // присваиваем Группу
                        QTYwarehouse    = price[i, 7],                          // присваиваем Количество на складах (Северянин + Пушкарев)
                        QTYstore        = price[i, 9],                          // присваиваем Количество в магазине
                        ShortTitle      = price[i, 2],                          // присваиваем Краткое наименование
                        Language        = price[i, 15],                         // присваиваем Язык
                        Age             = price[i, 16],                         // присваиваем Рекомендуемый возраст
                        Year            = price[i, 17],                         // присваиваем Год
                        Author          = price[i, 18],                         // присваиваем Автора
                        Catalog1        = price[i, 19],                         // присваиваем Категорию каталога 1
                        Catalog2        = price[i, 20],                         // присваиваем Категорию каталога 2
                        Catalog3        = price[i, 21],                         // присваиваем Категорию каталога 3
                        Catalog4        = price[i, 22],                         // присваиваем Категорию каталога 4
                        Catalog5        = price[i, 23]                          // присваиваем Категорию каталога 5
                    });
                }
                Console.WriteLine($"В прайс добавлено {i+1} из {rows}");
            }

            // Сортируем наш прайс по полю ShortTitle
            priceList = priceList.OrderBy(item => item.ShortTitle).ToList();

            // Добавляем нумерацию
            int count = 1;
            foreach (PriceModel item in priceList)
            {
                item.Number = count;
                count++;
            }

            // Сохранияем как в Excel файл
            SaveAsExcel(priceList);

            // Добавляем в архив
            WorkWithFile.AddPriceToZIP(destinationPath);
        }
        private void SaveAsExcel(List<PriceModel> priceList)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(DateTime.Now.ToString("dd.MM.yyyy"));

            // Добавляем шапку в первую строку
            worksheet.Cells["A1"].Value = "#";
            worksheet.Cells["B1"].Value = "ISBN";
            worksheet.Cells["C1"].Value = "Наименование товара";
            worksheet.Cells["D1"].Value = "Цена с НДС";
            worksheet.Cells["E1"].Value = "НДС";
            worksheet.Cells["F1"].Value = "Группа товара";
            worksheet.Cells["G1"].Value = "Кол-во на складе";
            worksheet.Cells["H1"].Value = "Кол-во в магазине";
            worksheet.Cells["I1"].Value = "Краткое наименование";
            worksheet.Cells["J1"].Value = "Язык";
            worksheet.Cells["K1"].Value = "Рекомендованный возраст";
            worksheet.Cells["L1"].Value = "Год";
            worksheet.Cells["M1"].Value = "Автор";
            worksheet.Cells["N1"].Value = "Категория каталога 1";
            worksheet.Cells["O1"].Value = "Категория каталога 2";
            worksheet.Cells["P1"].Value = "Категория каталога 3";
            worksheet.Cells["Q1"].Value = "Категория каталога 4";
            worksheet.Cells["R1"].Value = "Категория каталога 5";

            // Добавляем данные из priceList начиная со второй строки
            worksheet.Cells["A2"].LoadFromCollection(priceList);

            // Устанавливаем ширину столбцов, кроме последнего ("Краткое наименование")
            worksheet.Column(1).AutoFit();      // #
            worksheet.Column(2).Width = 16;     // ISBN
            worksheet.Column(3).Width = 110;    // Наименование товара
            worksheet.Column(4).Width = 15;     // Цена с НДС
            worksheet.Column(5).Width = 7;      // НДС
            worksheet.Column(6).Width = 22;     // Группа товара
            worksheet.Column(7).Width = 20;     // Кол-во на складе
            worksheet.Column(8).Width = 20;     // Кол-во в магазине
            worksheet.Column(9).Width = 25;     // Краткое наименование
            worksheet.Column(10).Width = 25;    // Язык
            worksheet.Column(11).Width = 25;    // Рекомендуемый возраст
            worksheet.Column(12).AutoFit();     // Год
            worksheet.Column(13).Width = 25;    // Автор
            worksheet.Column(14).Width = 25;    // Категория каталога 1
            worksheet.Column(15).Width = 25;    // Категория каталога 2
            worksheet.Column(16).Width = 25;    // Категория каталога 3
            worksheet.Column(17).Width = 25;    // Категория каталога 4
            worksheet.Column(18).Width = 25;    // Категория каталога 5

            // Устанавливаем границы, автофильтр, жирный шрифт для шапки, закрепляем первую строку, 
            // а также меняем цифровой формат для столбца с ценами
            worksheet.Column(4).Style.Numberformat.Format = "0.00";
            worksheet.View.FreezePanes(2, 1);
            worksheet.Cells["A1:R1"].Style.Font.Bold    = true;
            worksheet.Cells["A1:R1"].AutoFilter         = true;
            worksheet.Cells["A1:R" + (priceList.Count + 1)].Style.Border.Top.Style      = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:R" + (priceList.Count + 1)].Style.Border.Right.Style    = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:R" + (priceList.Count + 1)].Style.Border.Bottom.Style   = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:R" + (priceList.Count + 1)].Style.Border.Left.Style     = ExcelBorderStyle.Thin;

            // Сохраняем файл в Excel
            FileInfo fi = new FileInfo(destinationPath);
            excelPackage.SaveAs(fi);
        }  
    }
}
