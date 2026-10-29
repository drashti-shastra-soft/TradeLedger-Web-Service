using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.Utils;

namespace TradeLedgerWebService.Repository
{
    public class ContractRepository : IContractRepository
    {
        public Domain.ContractLister GetAll(Domain.ContractLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from co in context.Contracts
                                join sc in context.Clients on co.SellerClientId equals sc.Id
                                join bc in context.Clients on co.BuyerClientId equals bc.Id
                                join c in context.Companies on co.CompanyId equals c.Id
                                join bb in context.Brokers on co.BuyerBrokerId equals bb.Id into joinCBB
                                from bb in joinCBB.DefaultIfEmpty()
                                join sb in context.Brokers on co.SellerBrokerId equals sb.Id into joinCSB
                                from sb in joinCSB.DefaultIfEmpty()
                                join at in context.AuditTrails on co.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = co.Id,
                                    CompanyId = co.CompanyId,
                                    SellerClientId = co.SellerClientId,
                                    BuyerClientId = co.BuyerClientId,
                                    BuyerBrokerId = co.BuyerBrokerId,
                                    SellerBrokerId = co.SellerBrokerId,
                                    //SellerBrokerage = co.SellerBrokerage,
                                    //BuyerBrokerage = co.BuyerBrokerage,
                                    IsActive = co.IsActive,
                                    Company = c.Name,
                                    SellerClient = sc.Name,
                                    SellerBroker = bb.Name,
                                    BuyerClient = bc.Name,
                                    BuyerBroker = bb.Name,

                                    // Aggregate brokerage from ContractItems
                                    SellerBrokerage = context.ContractItems.Where(ci => ci.ContractId == co.Id).Sum(ci => (decimal?)ci.SellerBrokerage) ?? 0,

                                    BuyerBrokerage = context.ContractItems.Where(ci => ci.ContractId == co.Id).Sum(ci => (decimal?)ci.BuyerBrokerage) ?? 0

                                };

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.SellerClient.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.SellerClient) && x.SellerClient.ToUpper().Contains(criteria.SellerClient.ToUpper()));

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }

                    // Switch to in-memory for complex string filters
                    var efContract = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efContract.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efContract = efContract.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Contract>>(JsonSerializer.Serialize(efContract.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Contract Upsert(Domain.Contract mContract)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efContract = context.Contracts.FirstOrDefault(x => x.Id == mContract.Id);
                    if (efContract == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mContract.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efContract = new Persistence.Contract
                        {
                            AuditId = audit.Id
                        };
                        context.Contracts.Add(efContract);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efContract.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mContract.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efContract.CompanyId = mContract.CompanyId;
                    efContract.SellerClientId = mContract.SellerClientId;
                    efContract.SellerBrokerId = mContract.SellerBrokerId;
                    efContract.BuyerClientId = mContract.BuyerClientId;
                    efContract.BuyerBrokerId = mContract.BuyerBrokerId;
                    efContract.SellerBrokerage = mContract.SellerBrokerage;
                    efContract.BuyerBrokerage = mContract.BuyerBrokerage;
                    efContract.IsActive = mContract.IsActive;

                    context.SaveChanges();
                    mContract.Id = efContract.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mContract;
        }

        public Domain.Contract Get(int id)
        {
            var mContract = new Domain.Contract();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efContract = (from co in context.Contracts
                                      where co.Id == id
                                      select new
                                      {
                                          Id = co.Id,
                                          CompanyId = co.CompanyId,
                                          SellerClientId = co.SellerClientId,
                                          BuyerClientId = co.BuyerClientId,
                                          BuyerBrokerId = co.BuyerBrokerId,
                                          SellerBrokerId = co.SellerBrokerId,
                                          SellerBrokerage = co.SellerBrokerage,
                                          BuyerBrokerage = co.BuyerBrokerage,
                                          IsActive = co.IsActive,
                                      }).FirstOrDefault();

                    if (efContract != null)
                    {
                        mContract = JsonSerializer.Deserialize<Domain.Contract>(JsonSerializer.Serialize(efContract));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mContract;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efContract = context.Contracts.FirstOrDefault(x => x.Id == id);
                    if (efContract != null)
                    {
                        var audit = context.AuditTrails.Find(efContract.AuditId);
                        if (audit != null)
                        {
                            audit.DeletedBy = loginUserId;
                            audit.DeletedDate = DateTime.Now;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
