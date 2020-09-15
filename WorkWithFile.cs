using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace AutoPrice
{
    static class WorkWithFile
    {
        public static string[] OpenFile(string pricelistPath)
        {
            string[] fileText = null;

            for (int i = 0; i < 10; i++)
            {
                if (File.Exists(pricelistPath))
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    fileText = File.ReadAllLines(pricelistPath, Encoding.GetEncoding(866));
                    return fileText;
                }
                else
                {
                    Console.WriteLine($"{i + 1} Попытка открыть исходный прайс-лист.");
                    Thread.Sleep(1000);
                }
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

            // Удаляем старый прайс-лист
            Console.WriteLine("Удаляю старый прайс");
            RemoveOldPrice(directory);

            // Загружаем архив на FTP
            Console.WriteLine("Отправляю архив на FTP");
            UploadToFTP(zipPath, directory);
        }
        public static void UploadToFTP(string zipPath, string directory)
        {
            string[] login_pass = File.ReadAllLines(directory + @"\dailyUpload\log.txt", Encoding.UTF8);

            // Создаем объект FtpWebRequest - он указывает на файл, который будет создан
            FtpWebRequest request   = (FtpWebRequest)WebRequest.Create("ftp://31.177.95.151/files/relod_price.zip"); // "ftp://ftp.relod.nichost.ru/files/relod_price.zip"
            request.Credentials     = new NetworkCredential(login_pass[0], login_pass[1]);

            // Устанавливаем метод на загрузку файлов
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Создаем поток для загрузки файла
            FileStream fs           = new FileStream(zipPath, FileMode.Open);
            byte[] fileContents     = new byte[fs.Length];
            fs.Read(fileContents, 0, fileContents.Length);
            fs.Close();

            // Пишем считанный в массив байтов файл в выходной поток
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
        }
        private static void RemoveOldPrice(string path)
        {
            string oldPrice = @"\Price roznitca " + DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy") + ".xlsx";
            if (File.Exists(path + oldPrice))
            {
                File.Delete(path + oldPrice);
            }
        }
    }
}
