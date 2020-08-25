using System.IO;
using System.IO.Compression;
using System.Text;

namespace AutoPrice
{
    static class WorkWithFile
    {
        public static string[] OpenFile(string pricelistPath)
        {
            string[] fileText   = null;

            if (File.Exists(pricelistPath))
            {
                fileText = File.ReadAllLines(pricelistPath, Encoding.UTF8);
                return fileText;
            }

            return fileText;
        }
        public static void AddPriceToZIP(string destinationPath)
        {
            string temp             = @"\_temp\";                                                         // Имя временной папки, по завершении работы она будет удалена
            string directory        = destinationPath.Remove(destinationPath.LastIndexOf("\\"));          // Директория, в которой был сохранен прайс (берем путь, и из него удаляем имя файла)
            string fileName         = destinationPath.Substring(destinationPath.LastIndexOf("\\") + 1);   // Выделяем имя файла. Нужно чтобы скопировать файл во временную папку
            string startPath        = directory + temp;                                                   // Путь к архивируемой папке
            string zipPath          = directory + @"\relod_price.zip";                                    // Полный путь к выходному файлу
            string abbreviations    = @"\Список сокращений.doc";                                          // Файл со списком сокращений

            // Создаем временную скрытую папку "_temp" и копируем туда прайс и файл "Список сокращений.doc"
            Directory.CreateDirectory(directory + temp);
            DirectoryInfo hideFolder    = new DirectoryInfo(directory + temp);
            hideFolder.Attributes       = FileAttributes.Hidden;

            File.Copy(destinationPath, directory + temp + fileName, true);
            File.Copy(directory + abbreviations, directory + temp + abbreviations, true);

            // Удаляем старый архив и создаем новый
            File.Delete(zipPath);
            ZipFile.CreateFromDirectory(startPath, zipPath);

            // Удаляем временную папку
            Directory.Delete(startPath, true);
        }
    }
}
