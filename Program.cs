using AutoPrice.Model;
using AutoPrice.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var errorLog = new ErrorLogging();
                var config = new Config(errorLog);
                new AdditionalInfo(config);
                new PriceModel(config);

                var timeNow = DateTime.Now;
                var timeToRun = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day + 1, config.HourToStart, config.MinuteToStart, 0);
                TimeSpan delayTime = timeToRun - timeNow;
                Task.Delay(delayTime).ContinueWith(x => SomeFunc(config, errorLog)).Wait();
            }
        }

        private static void SomeFunc(Config config, ErrorLogging errorLog)
        {
            try
            {
                var priceList = new PriceList(config, errorLog).MakePriceList();
                new MakeExcelFile(priceList, config, errorLog).SavePriceAsExcel();
                new MakeArchiveFile(config, errorLog).CreateArchiveFile();
                new UploadToFTP(config, errorLog).UploadPrice();
                new RemoveOldPrice(config, errorLog).Remove();
            }
            catch (Exception ex)
            {
                errorLog.ErrorMessage = $"{DateTime.Now} : При поптыке сформировать прайс-лист произошла ошибка \n{ex}";
            };
        }
    }
}
