using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TradeLedgerWebService.Infrastructure.IService;

namespace TradeLedgerWebService.Service
{
    public class EmailSenderService : IEmailSenderService
    {
       
        //private readonly string _host = ConfigurationManager.AppSettings["SmtpHost"];
        //private readonly int _port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        //private readonly bool _enableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"]);
        //private readonly string _username = ConfigurationManager.AppSettings["SmtpUsername"];
        //private readonly string _password = ConfigurationManager.AppSettings["SmtpPassword"];
        //private readonly string _fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
        //private readonly string _fromName = ConfigurationManager.AppSettings["SmtpFromName"];
        //private readonly string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
        //private static readonly object _logLock = new object();
        //private static readonly object _errorLogLock = new object();

        //public async Task SendEmailAsync(string subject, string htmlBody, string toEmail, int maxRetries = 3)
        //{
        //    int attempt = 0;
        //    Exception lastException = null;

        //    while (attempt < maxRetries)
        //    {
        //        try
        //        {
        //            using (var smtpClient = new SmtpClient(_host, _port))
        //            {
        //                smtpClient.Timeout = 15000;
        //                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //                smtpClient.UseDefaultCredentials = false;
        //                smtpClient.Credentials = new NetworkCredential(_username, _password);
        //                smtpClient.EnableSsl = _enableSsl;

        //                using (var mailMessage = new MailMessage())
        //                {
        //                    mailMessage.From = new MailAddress(_fromEmail, _fromName);
        //                    mailMessage.To.Add(toEmail);
        //                    mailMessage.Subject = subject;
        //                    mailMessage.Body = htmlBody;
        //                    mailMessage.IsBodyHtml = true;

        //                    await smtpClient.SendMailAsync(mailMessage);
        //                }
        //            }

        //            lock (_logLock)
        //            {
        //                File.AppendAllText(Path.Combine(projectFolder, "Log.txt"), $"{DateTime.Now}: Sent to {toEmail}{Environment.NewLine}");
        //            }
        //            return; // success
        //        }
        //        catch (Exception ex)
        //        {
        //            lastException = ex;
        //            attempt++;

        //            lock (_errorLogLock)
        //            {
        //                File.AppendAllText(Path.Combine(projectFolder, "ErrorLog.txt"), $"{DateTime.Now}: Attempt {attempt} failed - {ex.Message}{Environment.NewLine}");
        //            }

        //            await Task.Delay(1000 * attempt); // exponential backoff
        //        }
        //    }
        //    throw new InvalidOperationException($"Failed to send email after {maxRetries} attempts.", lastException);
        //}

        public void SendEmail(string htmlBody, string toEmail, string subject)
        {
            try
            {
                var smtpClient = new SmtpClient("mail.mitsolutions.in")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("mail@mitsolutions.in", "mAiL#M@20!?"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mail@mitsolutions.in", "TradeLedger Support"),
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
