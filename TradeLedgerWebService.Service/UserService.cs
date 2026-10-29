using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using TradeLedgerWebService.Infrastructure.Utils;

namespace TradeLedgerWebService.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IFileReaderService _fileReaderService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ICompanyRepository _companyRepository;

        public UserService(IUserRepository repo, IFileReaderService fileReaderService, IEmailSenderService emailSenderService, ICompanyRepository companyRepository)
        {
            _repo = repo;
            _fileReaderService = fileReaderService;
            _emailSenderService = emailSenderService;
            _companyRepository = companyRepository;
        }

        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.User Upsert(Domain.User mUser)
        {
            bool isNewUser = false;

            if (mUser == null)
                throw new APIRequestFailedException("User object must not be null.");
            else if (mUser.Id == default(int))
                isNewUser = true;

            SetUserPassword(mUser);

            var mCreatedUser = _repo.Upsert(mUser);

            if (IsValidUser(mCreatedUser) && isNewUser)
            {
                var mCompany = _companyRepository.Get(mCreatedUser.CompanyId);
                SendEmailOnCreateUser(mCreatedUser, mCompany);
            }

            return mCreatedUser;
        }
        //public Domain.User Upsert(Domain.User mUser)
        //{
        //    if (mUser == null)
        //        throw new APIRequestFailedException("User object must not be null.");

        //    SetUserPassword(mUser);

        //    var mCreatedUser = _repo.Upsert(mUser);
        //    if (IsValidUser(mCreatedUser))
        //    {
        //        var mCompany = _companyRepository.Get(mCreatedUser.CompanyId);
        //        SendEmailOnCreateUser(mCreatedUser, mCompany);
        //    }

        //    return mCreatedUser;
        //}

        public Domain.User Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }


        #region Private Methods
        private void SendEmailOnCreateUser(Domain.User mUser, Domain.Company mCompany)
        {
            string content = _fileReaderService.ReadFileContent("~/EmailTemplates/SendCredentialsOnCreateUser.html");
            string finalContent = PopulateEmailTemplate(content, mUser, mCompany);

            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        await _emailSenderService.SendEmailAsync(finalContent, mUser.EmailAddress, "Welcome to Trade Ledger!");
            //    }
            //    catch (Exception ex)
            //    {
            //        System.IO.File.AppendAllText("ErrorLog.txt", $"{DateTime.Now}: Background send failed - {ex.Message}{Environment.NewLine}");
            //    }
            //});

            _emailSenderService.SendEmail(finalContent, mUser.EmailAddress, $"Welcome to {mCompany.Name} – Your Account Details Inside");

        }

        private string PopulateEmailTemplate(string template, Domain.User mUser, Domain.Company mCompany)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{{CompanyName}}", mCompany.Name },
                { "{{FirstName}}", mUser.FirstName },
                { "{{Username}}", mUser.EmailAddress },
                { "{{Password}}", mUser.PlainTextPassword},
                { "{{Year}}", DateTime.Now.Year.ToString() },
                { "{{CompanyLogo}}", mCompany.Logo }
            };

            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }

            return template;
        }

        private void SetUserPassword(Domain.User user)
        {
            user.PlainTextPassword = StaticMethods.GeneratePassword(8);
            var salt = ConfigurationManager.AppSettings.Get("Salt");
            user.Password = PasswordEncryptor.Encrypt(user.PlainTextPassword, salt);
        }

        private bool IsValidUser(Domain.User user)
        {
            return user != null && user.Id > 0;
        }

        #endregion
    }
}
