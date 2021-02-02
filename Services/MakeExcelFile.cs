using AutoPrice.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;

namespace AutoPrice.Services
{
    class MakeExcelFile
    {
        private readonly Config _config;
        private readonly ErrorLogging _error;

        public MakeExcelFile(Config config, ErrorLogging error)
        {
            _config = config;
            _error = error;
        }

        public void SavePriceAsExcel(List<PriceModel> priceList)
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
            int cellsCount = priceList.Count + 1;
            worksheet.Column(4).Style.Numberformat.Format = "0.00";
            worksheet.View.FreezePanes(2, 1);
            worksheet.Cells["A1:R1"].Style.Font.Bold = true;
            worksheet.Cells["A1:R1"].AutoFilter = true;
            worksheet.Cells["A1:R" + cellsCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:R" + cellsCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:R" + cellsCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:R" + cellsCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            // Сохраняем файл в Excel
            var path = _config.DestinationPath + _config.ExcelFileName;

            try
            {
                FileInfo fi = new FileInfo(path);
                excelPackage.SaveAs(fi);
            }
            catch (Exception ex)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке сохранить прайс-лист в Excel файл в папке {path} произошла непредвиденная ошибка \n{ex}";
            }
        }
    }
}
