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
            try
            {
                while (true)
                {
                    // Настраиваем таймер запуска
                    var timeNow = DateTime.Now;
                    var timeToRun = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day + 1, 6, 30, 0);
                    var delayTime = timeToRun - timeNow;
                    Task.Delay(delayTime).Wait();
                    // ==========================

                    var errorLog = new ErrorLogging();
                    var config = new Config(errorLog);
                    var addInfo = new AdditionalInfo(config);
                    var priceModel = new PriceModel(config);
                    var priceList = new PriceList(config, errorLog);
                    var makeExcel = new MakeExcelFile(config, errorLog);
                    var makeArchive = new MakeArchiveFile(config, errorLog);
                    var upload = new UploadToFTP(config, errorLog);
                    var remove = new RemoveOldPrice(config, errorLog);
                    var report = new EmailReport(config, errorLog);

                    if (!config.GetConfig())
                    {
                        return;
                    }

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
                    }

                    try
                    {
                        report.SendReport();
                    }
                    catch (Exception ex)
                    {
                        errorLog.ErrorMessage = $"{DateTime.Now} : При поптыке отправить письма произошла ошибка \n{ex}";
                    }
                }
            }
            catch (Exception ex) 
            {
                var error = new ErrorLogging();
                error.ErrorMessage = $"{DateTime.Now} : Произошла глобальная ошибка, для которой не был предусмотрен обработчик ошибок. \n{ex}";
            }
        }
    }
}
