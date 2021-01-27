using AutoPrice.Model;
using System.Globalization;

namespace AutoPrice
{
    public class PriceModel
    {
        public int Number { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public double VAT { get; set; }
        public string Group { get; set; }
        public double QTYwarehouse { get; set; }
        public double QTYstore { get; set; }
        public string ShortTitle { get; set; }
        public string Language { get; set; }
        public string Age { get; set; }
        public string Year { get; set; }
        public string Author { get; set; }
        public string Catalog1 { get; set; }
        public string Catalog2 { get; set; }
        public string Catalog3 { get; set; }
        public string Catalog4 { get; set; }
        public string Catalog5 { get; set; }

        private static Config _config;

        public PriceModel() { }

        public PriceModel(Config config) => _config = config;

        public static PriceModel GetPriceList(string row)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture(_config.SpeceficCulture);

            string[] temp = row.Split('\t');

            double.TryParse(temp[6], NumberStyles.Any, culture, out double price);
            double.TryParse(temp[4], NumberStyles.Any, culture, out double vat);
            double.TryParse(temp[7], NumberStyles.Any, culture, out double QTYwarehouse);
            double.TryParse(temp[9], NumberStyles.Any, culture, out double QTYstore);

            return new PriceModel()
            {
                ISBN            = temp[1],
                Title           = temp[14],
                Price           = price,
                VAT             = vat,
                Group           = temp[3],
                QTYwarehouse    = QTYwarehouse,
                QTYstore        = QTYstore,
                ShortTitle      = temp[2]
            };
        }
    }
}
