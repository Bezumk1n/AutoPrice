using System.Linq;

namespace AutoPrice.Model
{
    class AdditionalInfo
    {
        public string ISBN { get; set; }
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

        public AdditionalInfo() { }
        public AdditionalInfo(Config config) => _config = config;

        public static AdditionalInfo GetAdditionalInfo(string row)
        {
            // Разбиваем входящую строку на массив
            string[] currentRow = row.Split('\t');

            // От полученного массива нам нужен пятый столбец, в котором указаны категории.
            // Бьем этот столбец и получаем еще один массив с разделенными категориями
            string[] temp = currentRow[5].Split(';');

            // Создаем временный массив, в него мы будем зписывать категории, отвечающие требованиям
            string[] tempCatalogs = new string[temp.Length];

            // Мы знаем, что в итоге может быть выделено максимум пять категорий, поэтому сразу создаем под них массив нужного размера
            string[] catalogs = new string[5];

            // Проходим циклом по массиву с категориями, чтобы выделить строки с категориями отвечающие требованиям
            for (int i = 0; i < temp.Length; i++)
            {
                // Проверяем, содержит ли текущая строка в массиве категорию из стоп-листа
                bool check = true;
                for (int j = 0; j < _config.ExceptionCategories.Length; j++)
                {
                    // Если содержит, то игнорируем эту строку и смотрим следующую
                    if (temp[i].Contains(_config.ExceptionCategories[j]))
                    {
                        check = false;
                        break;
                    }
                }
                // Если в текущей строке массива с категориями не оказалось категории из стоп-листа, тогда записываем эту строку во временный массив с годными категориями
                if (check)
                {
                    tempCatalogs[i] = temp[i];
                }
            }

            // Теперь нам необходимо из годных строк с категориями выбрать самую полную (с максимальным количеством разделителя категории "/") и записать ее в конечный массив
            int checkSumm = 0;
            if (tempCatalogs.Length > 0)
            {
                for (int i = 0; i < tempCatalogs.Length; i++)
                {
                    if (tempCatalogs[i]?.IndexOf('/') > 0)
                    {
                        int count = tempCatalogs[i].Count(f => f == '/');
                        if (count >= checkSumm)
                        {
                            checkSumm = count;
                            string[] catalogGroups = tempCatalogs[i].Split('/');
                            for (int j = 0; j < catalogGroups.Length; j++)
                            {
                                catalogs[j] = catalogGroups[j].Trim();
                            }
                        }
                    }
                }
                // Предполагается что в массиве с годными строками может оказаться всего 1 строка без разделителя "/"
                // Для этого делаем проверку и записывем единственную строку в конечный массив
                if (catalogs[0] == null)
                {
                    catalogs[0] = tempCatalogs[0];
                }
            }

            return new AdditionalInfo()
            {
                ISBN        = currentRow[0],
                Language    = currentRow[1],
                Age         = currentRow[2],
                Year        = currentRow[3],
                Author      = currentRow[4],
                Catalog1    = catalogs[0],
                Catalog2    = catalogs[1],
                Catalog3    = catalogs[2],
                Catalog4    = catalogs[3],
                Catalog5    = catalogs[4]
            };
        }
    }
}
