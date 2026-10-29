using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TradeLedgerWebService.Infrastructure.IRepository;

namespace TradeLedgerWebService.Repository
{
    public class ContractItemRepository : IContractItemRepository
    {
        public Domain.ContractItemLister GetAll(Domain.ContractItemLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from ci in context.ContractItems
                                join at in context.AuditTrails on ci.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = ci.Id,
                                    ContractId = ci.ContractId,
                                    ItemId = ci.ItemId,
                                    Qty = ci.Qty,
                                    Rate = ci.Rate,
                                    SellerBrokerage = ci.SellerBrokerage,
                                    BuyerBrokerage = ci.BuyerBrokerage,
                                };

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.ContractId > 0)
                            query = query.Where(x => x.ContractId == criteria.ContractId);

                        if (criteria.ItemId > 0)
                            query = query.Where(x => x.ItemId == criteria.ItemId);
                    }

                    var efContractItem = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efContractItem.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efContractItem = efContractItem.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.ContractItem>>(JsonSerializer.Serialize(efContractItem.ToList()));

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.ContractItem Upsert(Domain.ContractItem mContractItem)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efContractItem = context.ContractItems.FirstOrDefault(x => x.Id == mContractItem.Id);
                    if (efContractItem == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mContractItem.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efContractItem = new Persistence.ContractItem
                        {
                            AuditId = audit.Id
                        };
                        context.ContractItems.Add(efContractItem);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efContractItem.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mContractItem.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efContractItem.ContractId = mContractItem.ContractId;
                    efContractItem.ItemId = mContractItem.ItemId;
                    efContractItem.Qty = mContractItem.Qty;
                    efContractItem.Rate = mContractItem.Rate;
                    efContractItem.SellerBrokerage = mContractItem.SellerBrokerage;
                    efContractItem.BuyerBrokerage = mContractItem.BuyerBrokerage;

                    context.SaveChanges();

                    mContractItem.Id = efContractItem.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mContractItem;
        }

        public List<Domain.ContractItem> Get(int id)
        {
            var mContractItemItem = new List<Domain.ContractItem>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efContractItem = (from ci in context.ContractItems
                                          where ci.ContractId == id
                                          select new
                                          {
                                              Id = ci.Id,
                                              ContractId = ci.ContractId,
                                              ItemId = ci.ItemId,
                                              Qty = ci.Qty,
                                              Rate = ci.Rate,
                                              SellerBrokerage = ci.SellerBrokerage,
                                              BuyerBrokerage = ci.BuyerBrokerage
                                          }).AsEnumerable();

                    mContractItemItem = JsonSerializer.Deserialize<List<Domain.ContractItem>>(JsonSerializer.Serialize(efContractItem.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mContractItemItem;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efContractItem = context.ContractItems.FirstOrDefault(x => x.Id == id);
                    if (efContractItem != null)
                    {
                        var audit = context.AuditTrails.Find(efContractItem.AuditId);
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
