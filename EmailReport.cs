﻿using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AutoPrice
{
    public static class EmailReport
    {
        static private string[] login_pass = File.ReadAllLines(@"\\Srv2008\relodobmen\Прайс-листы\dailyUpload\log.txt", Encoding.UTF8);
        static private string files = "<p>Список файлов в директории для контроля:<br>";
        static DirectoryInfo dirInfo = new DirectoryInfo(@"\\Srv2008\relodobmen\Прайс-листы\");

        static MailAddress to;
        static MailMessage mail;
        static SmtpClient smtp;
        static MailAddress from;

        static string subject = "Price Report";
        static string message =
                "<h4>Прайс-лист был сгенерирован и размещен в архиве на сайте <a href=\"http://www.relod.ru/company/publishers\">www.relod.ru/company/publishers</a></h4>" +
                "Прямая ссылка для скачивания прайса: <a href =\"http://www.relod.ru/files/relod_price.zip\">www.relod.ru/files/relod_price.zip</a><br>" +
                "<p>ЗАО РЕЛОД<br>" +
                DateTime.Now.ToString();
        public static void SendReport()
        {
            foreach (var item in dirInfo.GetFiles())
            {
                files += item.Name + "<br>";
            }

            from                = new MailAddress("stanislav.umnov@relod.ru", "RELOD Price Report");
            string[] toAdress   = new string[] { "umnov.msk@gmail.com" };

            for (int i = 0; i < toAdress.Length; i++)
            {
                to                  = new MailAddress(toAdress[i]);
                mail                = new MailMessage(from, to);
                mail.Subject        = subject;

                if (toAdress[i] == "umnov.msk@gmail.com")
                {
                    mail.Body = message + files;
                }
                else 
                {
                    mail.Body = message;
                }

                mail.IsBodyHtml     = true;
                smtp                = new SmtpClient("mail.relod.ru");
                smtp.Credentials    = new NetworkCredential(login_pass[2], login_pass[3]);
                smtp.Send(mail);
                Console.WriteLine($"Отправлено {i + 1} писем из {toAdress.Length}");
            }
        }
        public static void SendReport(string message)
        {
            foreach (var item in dirInfo.GetFiles())
            {
                files += item.Name + "<br>";
            }

            from                = new MailAddress("stanislav.umnov@relod.ru", "RELOD Price Report: ERROR");
            string[] toAdress   = new string[] { "umnov.msk@gmail.com" };

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