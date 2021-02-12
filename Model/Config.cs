using System;
using System.IO;

namespace AutoPrice.Model
{
    public class Config
    {
        public string PriceListFilePath { get; private set; }
        public int Encoding { get; private set; }
        public string AdditionalInfoFilePath { get; private set; }
        public string[] ExclusiveGroups { get; private set; }
        public string[] IgnorableGroups { get; private set; }
        public string[] ExceptionGroups { get; private set; }
        public string[] ExceptionCategories { get; private set; }
        public string SpeceficCulture { get; private set; }
        public string DestinationPath { get; private set; }
        public string ExcelFileName { get; private set; }
        public string ArchiveFileName { get; private set; }
        public string AbbreviationsPath { get; private set; }
        public string FtpAdress { get; private set; }
        public string Login { get; private set; }
        public string Pass { get; private set; }
        public string TempPath { get; private set; }
        public string[] ErrorReportMailRecipients { get; private set; }
        public string[] ReportMailRecipients { get; private set; }
        public string MailLogin { get; private set; }
        public string MailPass { get; private set; }

        private ErrorLogging _error;

        public Config(ErrorLogging error) => _error = error;

        public bool GetConfig()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@".\config\config.cfg"))
                {
                    var lines = sr.ReadToEnd().Split(Environment.NewLine);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].ToLower().StartsWith("pricelistpath"))
                        {
                            PriceListFilePath = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("additionalinfofilepath"))
                        {
                            AdditionalInfoFilePath = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("priceencoding"))
                        {
                            Encoding = int.Parse(lines[i].Substring(lines[i].IndexOf("=") + 1).Trim());
                        }
                        else if (lines[i].ToLower().StartsWith("speceficculture"))
                        {
                            SpeceficCulture = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("exclusivegroups"))
                        {
                            ExclusiveGroups = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim().Split(';');
                        }
                        else if (lines[i].ToLower().StartsWith("ignorablegroups"))
                        {
                            IgnorableGroups = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim().Split(';');
                        }
                        else if (lines[i].ToLower().StartsWith("exceptiongroups"))
                        {
                            ExceptionGroups = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim().Split(';');
                        }
                        else if (lines[i].ToLower().StartsWith("exceptioncategories"))
                        {
                            ExceptionCategories = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim().Split(';');
                        }
                        else if (lines[i].ToLower().StartsWith("destinationpath"))
                        {
                            DestinationPath = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("excelfilename"))
                        {
                            ExcelFileName = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();

                            if (ExcelFileName == "default")
                            {
                                ExcelFileName = $"Price roznitca {DateTime.Now.ToString("dd.MM.yyyy")}.xlsx";
                            }
                        }
                        else if (lines[i].ToLower().StartsWith("archivefilename"))
                        {
                            ArchiveFileName = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("abbreviationspath"))
                        {
                            AbbreviationsPath = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("ftpadress"))
                        {
                            FtpAdress = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("login"))
                        {
                            Login = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("pass"))
                        {
                            Pass = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("temp"))
                        {
                            TempPath = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("maillogin"))
                        {
                            MailLogin = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("mailpass"))
                        {
                            MailPass = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim();
                        }
                        else if (lines[i].ToLower().StartsWith("errorreportmailrecipients"))
                        {
                            ErrorReportMailRecipients = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim().Split(';');
                        }
                        else if (lines[i].ToLower().StartsWith("reportmailrecipients"))
                        {
                            ReportMailRecipients = lines[i].Substring(lines[i].IndexOf("=") + 1).Trim().Split(';');
                        }
                    }
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : Не найден файл конфигурации config.cfg в папке {Environment.CurrentDirectory}\\config";
            }
            catch (DirectoryNotFoundException)
            {
                _error.ErrorMessage = $"{DateTime.Now} : В папке {Environment.CurrentDirectory} не найдена папка config";
            }
            catch (Exception)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке обработать файл конфигурации config.cfg в папке {Environment.CurrentDirectory}\\config произошла непредвиденная ошибка";
            }
            return false;
        }
    }
}
