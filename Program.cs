using System;

namespace AutoPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            string pricelistPath    = @"\\Srv2008\relodobmen\Прайс-листы\dailyUpload\prices.txt";
            string exceptionPath    = @"\\Srv2008\relodobmen\Прайс-листы\dailyUpload\Exceptions.txt";
            string destinationPath  = @"\\Srv2008\relodobmen\Прайс-листы\" + "Price roznitca " + DateTime.Now.ToString("dd.MM.yyyy") + ".xlsx";
            string addInfoPath      = @"\\Srv2008\relodobmen\Прайс-листы\dailyUpload\additionalInfo.csv";
            bool fullPrice          = false;

            try
            {
                new DoPriceList(pricelistPath, exceptionPath, destinationPath, addInfoPath, fullPrice);
                Console.WriteLine("Отправляю отчет по почте");
                EmailReport.SendReport();
            }
            catch (Exception ex)
            {
                string message = "При попытке сгенерировать прайс-лист произошла ошибка.";
                EmailReport.SendReport(message + "\n" + ex);
            }
        }
    }
}
