namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IChangePasswordService
    {
        void Upsert(Domain.ChangePassword mChangePassword);
    }
}
