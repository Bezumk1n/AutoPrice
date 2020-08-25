using System;

namespace AutoPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            string pricelistPath    = @"C:\Users\MEsales4\Desktop\PriceList\Прайс-лист (наличие_ цены).txt";
            string exceptionPath    = @"C:\Users\MEsales4\Desktop\PriceList\Exceptions.txt";
            string destinationPath  = @"\\Srv2008\relodobmen\Прайс-листы\" + "Price roznitca " + DateTime.Now.ToString("dd.MM.yyyy") + ".xlsx";
            bool fullPrice          = false;
            new DoPriceList(pricelistPath, exceptionPath, destinationPath, fullPrice);
        }
    }
}
