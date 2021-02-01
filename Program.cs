﻿using AutoPrice.Model;
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
                var errorLog = new ErrorLogging();
                var config = new Config(errorLog);
                new AdditionalInfo(config);
                new PriceModel(config);
                var report = new EmailReport(config, errorLog);

                var timeNow = DateTime.Now;
                var timeToRun = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day + 1, config.HourToStart, config.MinuteToStart, 0);
                TimeSpan delayTime = timeToRun - timeNow;
                Task.Delay(delayTime).Wait();

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

                report.SendReport();
            }
        }
    }
}
