using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Configuration;

namespace TradeLedgerWebService.Service
{
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly IUserRepository _userRepo;

        public ChangePasswordService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public void Upsert(Domain.ChangePassword mChangePassword)
        {
            var currentPassword = _userRepo.GetPasswordBy(mChangePassword.UserId);
            if (currentPassword.IsNullOrEmpty())
                throw new APIRequestFailedException("Something is missing at your profile, please contact to administrator!");

            var encryptedNewPassword = PasswordEncryptor.Encrypt(mChangePassword.CurrentPassword, ConfigurationManager.AppSettings.Get("Salt"));

            if (currentPassword != encryptedNewPassword)
                throw new APIRequestFailedException("The current password you entered is incorrect.");

            if (currentPassword != PasswordEncryptor.Encrypt(mChangePassword.CurrentPassword, ConfigurationManager.AppSettings.Get("Salt")))
                throw new APIRequestFailedException("Current password is invalid!");

            _userRepo.SetPasswordBy(mChangePassword.UserId, PasswordEncryptor.Encrypt(mChangePassword.NewPassword, ConfigurationManager.AppSettings.Get("Salt")));
        }
    }
}
