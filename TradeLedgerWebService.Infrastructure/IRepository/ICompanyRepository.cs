using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface ICompanyRepository
    {
        List<Domain.Company> GetActiveCompanies();

        Domain.CompanyLister GetAll(Domain.CompanyLister mLister);

        Domain.Company Upsert(Domain.Company mCompany);

        Domain.Company Get(int id);

        void Delete(int id, int loginUserId);
    }
}
