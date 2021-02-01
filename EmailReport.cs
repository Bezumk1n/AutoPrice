using AutoPrice.Model;
using System;
using System.Net;
using System.Net.Mail;

namespace AutoPrice
{
    public class EmailReport
    {
        private readonly Config _config;
        private readonly ErrorLogging _error;

        public EmailReport() { }
        public EmailReport(Config config, ErrorLogging error)
        {
            _config = config;
            _error = error;
        }

        public void SendReport()
        {
            var smtp = new SmtpClient("mail.relod.ru");
            smtp.Credentials = new NetworkCredential(_config.MailLogin, _config.MailPass);
            var from = new MailAddress("stanislav.umnov@relod.ru", "RELOD Price Report");

            var subject = "Price Report";
            var message =
                "<h4>Прайс-лист был сгенерирован и размещен в архиве на сайте <a href=\"http://www.relod.ru/company/publishers\">www.relod.ru/company/publishers</a></h4>" +
                "Прямая ссылка для скачивания прайса: <a href =\"http://www.relod.ru/files/relod_price.zip\">www.relod.ru/files/relod_price.zip</a><br>" +
                "<p>ЗАО РЕЛОД<br>" +
                DateTime.Now.ToString();
            var errorMessage = "При подготовке прайс-листа произошла ошибка.";

            try
            {
                if (_error.isErrorOccured)
                {
                    foreach (var adress in _config.ErrorReportMailRecipients)
                    {
                        SendErrorReport(smtp, from, adress, subject, errorMessage);
                    }
                }
                else
                {
                    foreach (var adress in _config.ReportMailRecipients)
                    {
                        SendErrorReport(smtp, from, adress, subject, message);
                    }
                }
            }
            catch (Exception ex)
            {
                _error.ErrorMessage = $"{DateTime.Now} : При поптыке отправить письма произошла непредвиденная ошибка \n{ex}";
            }
        }

        private void SendErrorReport(SmtpClient smtp, MailAddress from, string adress, string subject, string message)
        {
            var to = new MailAddress(adress);
            var mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            if (_error.isErrorOccured)
            {
                mail.Attachments.Add(new Attachment(@".\errorLog.txt"));
            }
            smtp.Send(mail);
        }
    }
}
