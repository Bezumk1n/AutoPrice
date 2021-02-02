using AutoPrice.Model;
using System;
using System.IO;
using System.Net;

namespace AutoPrice.Services
{
    class UploadToFTP
    {
        private readonly Config _config;
        private readonly ErrorLogging _error;

        public UploadToFTP(Config config, ErrorLogging error)
        {
            _config = config;
            _error = error;
        }

        public void UploadPrice()
        {
            try
            {
                // Создаем объект FtpWebRequest - он указывает на файл, который будет создан
                var connectionPath = $"{_config.FtpAdress}{_config.ArchiveFileName}";
                var request = (FtpWebRequest)WebRequest.Create(connectionPath);
                request.Credentials = new NetworkCredential(_config.Login, _config.Pass);

                // Устанавливаем метод на загрузку файлов
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Создаем поток для загрузки файла
                var fs = new FileStream($"{_config.TempPath}\\{_config.ArchiveFileName}", FileMode.Open);
                var fileContents = new byte[fs.Length];
                fs.Read(fileContents, 0, fileContents.Length);
                fs.Close();

                // Пишем считанный в массив байтов файл в выходной поток
                var requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
            }
            catch (Exception ex)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке отправить архив на FTP произошла непредвиденная ошибка \n{ex}";
            }
        }
    }
}
