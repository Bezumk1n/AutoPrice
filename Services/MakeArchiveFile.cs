using AutoPrice.Model;
using System;
using System.IO;
using System.IO.Compression;

namespace AutoPrice.Services
{
    class MakeArchiveFile
    {
        private readonly Config _config;
        private readonly ErrorLogging _error;

        public MakeArchiveFile(Config config, ErrorLogging error)
        {
            _config = config;
            _error = error;
        }

        public void CreateArchiveFile()
        {
            // Сначала создадим временную папку в которую скопируем прайс, файл с дополнительной информацией и создадим архив
            try
            {
                var tempPath = Directory.CreateDirectory(_config.TempPath);
                tempPath.Attributes = FileAttributes.Hidden;
                var archPath = Directory.CreateDirectory(tempPath + "\\archTemp");

                // Копируем файл прайс-листа
                File.Copy($"{_config.DestinationPath}\\{_config.ExcelFileName}", $"{archPath.FullName}\\{_config.ExcelFileName}", true);
                // Копируем файл со списком сокращений
                File.Copy($"{_config.AbbreviationsPath}", $"{archPath.FullName}\\{_config.AbbreviationsPath.Substring(_config.AbbreviationsPath.LastIndexOf("\\"))}", true);

                // Создаем архив в папке archTemp
                ZipFile.CreateFromDirectory(archPath.FullName, $"{tempPath}\\{_config.ArchiveFileName}");
            }
            catch (Exception ex)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке создать архив произошла непредвиденная ошибка \n{ex}";
            }
        }
    }
}
