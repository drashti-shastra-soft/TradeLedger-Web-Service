namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IUserRepository
    {
        Domain.UserLister GetAll(Domain.UserLister mLister);

        Domain.User Upsert(Domain.User mUser);

        Domain.User Get(int id);

        void Delete(int id, int loginUserId);

        Domain.User GetBy(string emailAddress);

        string SetPasswordBy(int id, string password);

        string GetPasswordBy(int id);
    }
}
