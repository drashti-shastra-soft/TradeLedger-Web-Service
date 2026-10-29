using System.Threading.Tasks;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IEmailSenderService
    {
        void SendEmail(string htmlBody, string toEmail, string subject);


        //Task SendEmailAsync(string subject, string htmlBody, string toEmail, int maxRetries = 3);
    }
}
