using AutoPrice.Model;
using AutoPrice.Services;
using System;

namespace AutoPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            var errorLog = new ErrorLogging();
            var config = new Config(errorLog);
            new PriceModel(config);
            new AdditionalInfo(config);

            try
            {
                var priceList = new PriceList(config, errorLog).DoPriceList();
                new MakeExcelFile(priceList, config, errorLog).SavePriceAsExcel();
                new MakeArchiveFile(config, errorLog).CreateArchiveFile();
                new UploadToFTP(config, errorLog).UploadPrice();
                new RemoveOldPrice(config, errorLog).Remove();
            }
            catch (Exception ex)
            {
                errorLog.ErrorMessage = $"{DateTime.Now} : При поптыке сформировать прайс-лист произошла ошибка \n{ex}";
            }
        }
    }
}
