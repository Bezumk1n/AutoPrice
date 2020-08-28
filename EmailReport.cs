using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AutoPrice
{
    public static class EmailReport
    {
        static string[] login_pass = File.ReadAllLines(@"\\Srv2008\relodobmen\Прайс-листы\\dailyUpload\log.txt", Encoding.UTF8);
        public static void SendReport()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"\\Srv2008\relodobmen\Прайс-листы\");
            string files = "<p>Список файлов в директории для контроля:<br>";
            foreach (var item in dirInfo.GetFiles())
            {
                files += item.Name + "<br>";
            }

            MailAddress to;
            MailMessage mail;
            SmtpClient smtp;

            MailAddress from    = new MailAddress("stanislav.umnov@relod.ru", "RELOD Price Report");
            string[] toAdress   = new string[] { "umnov.msk@gmail.com" };
            string subject      = "Price Report";
            string message      =
                "<h4>Прайс-лист был сгенерирован и размещен в архиве на сайте <a href=\"http://www.relod.ru/company/publishers\">www.relod.ru/company/publishers</a></h4>" +
                "Прямая ссылка для скачивания прайса: <a href =\"http://www.relod.ru/files/relod_price.zip\">www.relod.ru/files/relod_price.zip</a><br>";

            for (int i = 0; i < toAdress.Length; i++)
            {
                to                  = new MailAddress(toAdress[i]);
                mail                = new MailMessage(from, to);
                mail.Subject        = subject;
                mail.Body           = message + files;
                mail.IsBodyHtml     = true;
                smtp                = new SmtpClient("mail.relod.ru");
                smtp.Credentials    = new NetworkCredential(login_pass[2], login_pass[3]);
                smtp.Send(mail);
                Console.WriteLine($"Отправлено {i + 1} писем из {toAdress.Length}");
            }
        }
        public static void SendReport(string message)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"\\Srv2008\relodobmen\Прайс-листы\");
            string files = "<p>Список файлов в директории для контроля:<br>";
            foreach (var item in dirInfo.GetFiles())
            {
                files += item.Name + "<br>";
            }

            MailAddress to;
            MailMessage mail;
            SmtpClient smtp;

            MailAddress from    = new MailAddress("stanislav.umnov@relod.ru", "RELOD Price Report: ERROR");
            string[] toAdress   = new string[] { "umnov.msk@gmail.com" };
            string subject      = "Price Report";

            for (int i = 0; i < toAdress.Length; i++)
            {
                to                  = new MailAddress(toAdress[i]);
                mail                = new MailMessage(from, to);
                mail.Subject        = subject;
                mail.Body           = message + files;
                mail.IsBodyHtml     = true;
                smtp                = new SmtpClient("mail.relod.ru");
                smtp.Credentials    = new NetworkCredential(login_pass[2], login_pass[3]);
                smtp.Send(mail);
            }
        }
    }
}
