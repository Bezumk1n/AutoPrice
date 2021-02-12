using AutoPrice.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPrice
{
    class PriceList
    {
        private Config _config;
        private ErrorLogging _error; 

        public PriceList(Config config, ErrorLogging error)
        {
            this._config = config;
            this._error = error;
        }
 
        public List<ClientPriceModel> MakePriceList()
        {
            var priceListPath = _config.PriceListFilePath;
            var additionalListPath = _config.AdditionalInfoFilePath;
            var encoding = _config.Encoding;
            var ignorableGroups = _config.IgnorableGroups;
            var exceptionGroups = _config.ExceptionGroups;
            var exclusiveGroups = _config.ExclusiveGroups;

            var additionalInfo = new List<AdditionalInfo>();
            var priceList = new List<PriceModel>();
            var clientPriceList = new List<ClientPriceModel>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Читаем файл с основным прайс-листом
            var getPrice = Task.Run(() => priceList = GetDataFromPriceFile(priceListPath, encoding));

            // Читаем файл с дополнительной информацией
            var getAddInfo = Task.Run(() => additionalInfo = GetDataFromAddInfoFile(additionalListPath, encoding));

            // Ищем и исключаем из списка все позиции у которых:
            // 1. Группа равна группе исключения (регулируется в файле config.cfg)
            // 2. Цена равна 0
            // 3. Полное наименование начинается со слов "агентское вознаграждение"
            // 4. Наименование заканчивается на "op!" или "na!" и при этом остатки по складам равны 0
            // 5. Остатки по складам равны 0 и группа не равна игнорируемым группам (например группы Oxford, CLE, Express Publishing...)(регулируется в файле config.cfg)
            // Список сортируем

            getPrice.Wait();
            try
            {
                priceList = priceList
                .Except(priceList.Where(
                    item => exceptionGroups.Contains(item.Group)))
                .Except(priceList.Where(
                    item => item?.Price == 0))
                .Except(priceList.Where(
                    item => item.Title.ToLower().StartsWith("агентское вознаграждение")))
                .Except(priceList.Where(
                    item =>
                    item.ShortTitle.ToLower().EndsWith("op!") |
                    item.ShortTitle.ToLower().EndsWith("na!") &&
                    item.QTYwarehouse == 0 &&
                    item.QTYstore == 0))
                .Except(priceList.Where(
                    item => !ignorableGroups.Contains(item.Group) &&
                    item.QTYwarehouse == 0 &&
                    item.QTYstore == 0))
                .OrderBy(item => item.ShortTitle)
                .ToList();
            }
            catch (ArgumentNullException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При попытке обработать основной прайс-лист произошла ошибка. Список был пустым";
                return null;
            }
            catch (Exception)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При попытке обработать основной прайс-лист произошла непредвиденная ошибка";
                return null;
            }

            // Объединяем два списка в один
            getAddInfo.Wait();
            int row = 1;
            try
            {
                clientPriceList = priceList
                .GroupJoin(additionalInfo,
                    p => p.ISBN,
                    a => a.ISBN,
                    (price, addInfo) => new { price, addInfo })
                .SelectMany(temp => temp.addInfo.DefaultIfEmpty(),
                    (t_price, t_addinfo) => new ClientPriceModel
                    {
                        Number          = row++,
                        ISBN            = t_price.price.ISBN,
                        Title           = t_price.price.Title,
                        Price           = t_price.price.Price,
                        VAT             = t_price.price.VAT,
                        Group           = t_price.price.Group,
                        QTYwarehouse    = t_price.price.QTYwarehouse > 10 ? "Более 10 шт" : t_price.price.QTYwarehouse.ToString(),
                        QTYstore        = t_price.price.QTYstore > 10 ? "Более 10 шт" : t_price.price.QTYstore.ToString(),
                        // Возможно понадобится выборочное указание количества если группа равна группе из эксклюзивного списка
                        //QTYwarehouse    = exclusiveGroups.Any(x => x.Contains(t_price.price.Group)) && t_price.price.QTYwarehouse < 20 ? "Менее 20 шт" :
                        //                    t_price.price.QTYwarehouse > 10 ? "Более 10 шт" : t_price.price.QTYwarehouse.ToString(),
                        //QTYstore        = exclusiveGroups.Any(x => x.Contains(t_price.price.Group)) && t_price.price.QTYstore < 20 ? "Менее 20 шт" :
                        //                    t_price.price.QTYstore > 10 ? "Более 10 шт" : t_price.price.QTYstore.ToString(),
                        ShortTitle      = t_price.price.ShortTitle,
                        Language        = t_addinfo?.Language ?? string.Empty,
                        Age             = t_addinfo?.Age ?? string.Empty,
                        Year            = t_addinfo?.Year ?? string.Empty,
                        Author          = t_addinfo?.Author ?? string.Empty,
                        Catalog1        = t_addinfo?.Catalog1 ?? string.Empty,
                        Catalog2        = t_addinfo?.Catalog2 ?? string.Empty,
                        Catalog3        = t_addinfo?.Catalog3 ?? string.Empty,
                        Catalog4        = t_addinfo?.Catalog4 ?? string.Empty,
                        Catalog5        = t_addinfo?.Catalog5 ?? string.Empty
                    })
                .ToList();

                return clientPriceList;
            }
            catch (ArgumentNullException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При попытке объединить основной прайс-лист со списком дополнительной информации произошла ошибка. Список с дополнительной информацией был пустым";
                return null;
            }
            catch (Exception)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При попытке объединить основной прайс-лист со списком дополнительной информации произошла непредвиденная ошибка";
                return null;
            }
        }
        private List<PriceModel> GetDataFromPriceFile(string path, int encoding)
        {
            try
            {
                return File.ReadAllLines(path, Encoding.GetEncoding(encoding))
                    .Select(PriceModel.GetPriceList)
                    .ToList();
            }
            catch (FileNotFoundException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : Не найден файл прайс-листа в папке {path}";
                return null;
            }
            catch (PathTooLongException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : Указан слишком длинный путь до файла прайс-листа";
                return null;
            }
            catch (Exception ex)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке обработать файл прайс-листа в папке {path} произошла непредвиденная ошибка \n{ex}";
                return null;
            }
        }
        private List<AdditionalInfo> GetDataFromAddInfoFile(string path, int encoding)
        {
            try
            {
                return File.ReadAllLines(path, Encoding.GetEncoding(encoding))
                    .Select(AdditionalInfo.GetAdditionalInfo)
                    .ToList();
            }
            catch (FileNotFoundException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : Не найден файл с дополнительной информацией в папке {path}";
                return null;
            }
            catch (PathTooLongException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : Указан слишком длинный путь до файла с дополнительной информацией";
                return null;
            }
            catch (Exception ex)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке обработать файл с дополнительной информацией в папке {path} произошла непредвиденная ошибка \n{ex}";
                return null;
            }
        }
    }
}
