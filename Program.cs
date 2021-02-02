using AutoPrice.Model;
using AutoPrice.Services;
using System;
using System.Threading.Tasks;

namespace AutoPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var errorLog    = new ErrorLogging();
                var config      = new Config(errorLog);
                var addInfo     = new AdditionalInfo(config);
                var priceModel  = new PriceModel(config);
                var priceList   = new PriceList(config, errorLog);
                var makeExcel   = new MakeExcelFile(config, errorLog);
                var makeArchive = new MakeArchiveFile(config, errorLog);
                var upload      = new UploadToFTP(config, errorLog);
                var remove      = new RemoveOldPrice(config, errorLog);
                var report      = new EmailReport(config, errorLog);

                if (!config.GetConfig())
                {
                    return;
                }

                var timeNow = DateTime.Now;
                var timeToRun = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day + 1, config.HourToStart, config.MinuteToStart, 0);
                TimeSpan delayTime = timeToRun - timeNow;
                Task.Delay(delayTime).Wait();

                try
                {
                    var price = priceList.MakePriceList();
                    makeExcel.SavePriceAsExcel(price);
                    makeArchive.CreateArchiveFile();
                    upload.UploadPrice();
                    remove.Remove();
                }
                catch (Exception ex)
                {
                    errorLog.ErrorMessage = $"{DateTime.Now} : При поптыке сформировать прайс-лист произошла ошибка \n{ex}";
                };

                report.SendReport();
            }
        }
    }
}
