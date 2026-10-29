using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Configuration;

namespace TradeLedgerWebService.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IFileReaderService _fileReaderService;

        public ForgotPasswordService(IUserRepository userRepository, IEmailSenderService emailSenderService, IFileReaderService fileReaderService)
        {
            _userRepository = userRepository;
            _emailSenderService = emailSenderService;
            _fileReaderService = fileReaderService;
        }

        public void Initiate(Domain.ForgotPassword mForgotPassword)
        {
            if(mForgotPassword.EmailAddress.IsNullOrEmpty())
                throw new APIRequestFailedException("Email address is required!");

            var mUser = _userRepository.GetBy(mForgotPassword.EmailAddress);
            if (mUser == null || mUser.Id == default(int))
                throw new APIRequestFailedException("No user found with the provided email address.");

            mUser.PlainTextPassword = StaticMethods.GeneratePassword(8);
            mUser.Password = PasswordEncryptor.Encrypt(mUser.PlainTextPassword, ConfigurationManager.AppSettings.Get("Salt"));
            _userRepository.SetPasswordBy(mUser.Id, mUser.Password);
            SendEmailOnForgotPassword(mUser);

        }

        #region Private Methods
        private string PopulateEmailTemplateForResetPassword(string template, Domain.User user)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{{FirstName}}", user.FirstName },
                { "{{EmailAddress}}", user.EmailAddress },
                { "{{Password}}", user.PlainTextPassword},
            };
            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }
            return template;
        }

        private void SendEmailOnForgotPassword(Domain.User mUser)
        {
            string content = _fileReaderService.ReadFileContent("~/EmailTemplates/SendCredentialsOnForgotPassword.html");
            string finalContent = PopulateEmailTemplateForResetPassword(content, mUser);
            _emailSenderService.SendEmail("🔒 Reset Your Account Password", finalContent, mUser.EmailAddress);
        }
        #endregion
    }
}
