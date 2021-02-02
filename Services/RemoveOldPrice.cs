using AutoPrice.Model;
using System;
using System.IO;

namespace AutoPrice.Services
{
    class RemoveOldPrice
    {
        private readonly Config _config;
        private readonly ErrorLogging _error;

        public RemoveOldPrice(Config config, ErrorLogging error)
        {
            _config = config;
            _error = error;
        }

        public void Remove()
        {
            // Удаляем старый прайс
            var oldPrice = $"Price roznitca {DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy")}.xlsx";
            if (File.Exists($"{_config.DestinationPath}{oldPrice}"))
            {
                try
                {
                    File.Delete(_config.DestinationPath + oldPrice);
                }
                catch (UnauthorizedAccessException)
                {
                    _error.ErrorMessage = $"{DateTime.Now} : При поптыке удалить старый прайс-лист произошла ошибка. Файл открыт в другой программе.";
                }
                catch (Exception ex)
                {
                    _error.ErrorMessage = $"{DateTime.Now} : При поптыке удалить старый прайс-лист произошла непредвиденная ошибка \n{ex}";
                }
            }

            // Удаляем старый архив и копируем новый
            if (File.Exists($"{_config.DestinationPath}\\{_config.ArchiveFileName}"))
            {
                try
                {
                    File.Delete($"{_config.DestinationPath}\\{_config.ArchiveFileName}");
                    File.Copy($"{_config.TempPath}\\{_config.ArchiveFileName}", $"{_config.DestinationPath}\\{_config.ArchiveFileName}");
                }
                catch (UnauthorizedAccessException)
                {
                    _error.ErrorMessage = $"{DateTime.Now} : При поптыке удалить старый архив произошла ошибка. Файл открыт в другой программе.";
                }
                catch (Exception ex)
                {
                    _error.ErrorMessage = $"{DateTime.Now} : При поптыке удалить старый архив произошла непредвиденная ошибка \n{ex}";
                }
            }

            // Удаляем временную директорию
            if (Directory.Exists(_config.TempPath))
            {
                try
                {
                    Directory.Delete(_config.TempPath, true);
                }
                catch (Exception ex)
                {
                    _error.ErrorMessage = $"{DateTime.Now} : При поптыке удалить временную директорию произошла непредвиденная ошибка \n{ex}";
                }
            }
        }
    }
}
