using System;

namespace AutoPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            string pricelistPath    = @"\\Srv2008\relodobmen\Прайс-листы\dailyUpload\Прайс-лист (наличие_ цены).txt";
            string exceptionPath    = @"\\Srv2008\relodobmen\Прайс-листы\dailyUpload\Exceptions.txt";
            string destinationPath  = @"\\Srv2008\relodobmen\Прайс-листы\" + "Price roznitca " + DateTime.Now.ToString("dd.MM.yyyy") + ".xlsx";
            bool fullPrice          = false;

            try
            {
                new DoPriceList(pricelistPath, exceptionPath, destinationPath, fullPrice);
                EmailReport.SendReport();
            }
            catch 
            {
                string message = "При попытке сгенерировать прайс-лист произошла ошибка.";
                EmailReport.SendReport(message);
            }
        }
    }
}
