namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IForgotPasswordService
    {
        void Initiate(Domain.ForgotPassword mForgotPassword);
    }
}
