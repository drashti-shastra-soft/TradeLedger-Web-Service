using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class ItemRepository : IItemRepository
    {
        public Domain.ItemLister GetAll(Domain.ItemLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from i in context.Items
                                join c in context.Companies on i.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join u in context.Units on i.UnitId equals u.Id
                                join at in context.AuditTrails on i.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = i.Id,
                                    CompanyId = i.CompanyId,
                                    Code = i.Code,
                                    Name = i.Name,
                                    Unit = u.Name,
                                    Description = i.Description,
                                    HSNCode = i.HSNCode,
                                    UnitId = i.UnitId,
                                    AuditId = i.AuditId,
                                    IsActive = i.IsActive,
                                    Company = c.Name,
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.Code.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Code) && x.Code.ToUpper().Contains(criteria.Code.ToUpper()));

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.HSNCode.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.HSNCode) && x.HSNCode.ToUpper().Contains(criteria.HSNCode.ToUpper()));

                        if (criteria.Unit.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Unit) && x.Unit.ToUpper().Contains(criteria.Code.ToUpper()));


                    }
                    // Switch to in-memory for complex string filters
                    var efItem = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efItem.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efItem = efItem.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Item>>(JsonSerializer.Serialize(efItem.ToList()));

                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.Item Upsert(Domain.Item mItem)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efItem = context.Items.FirstOrDefault(x => x.Id == mItem.Id);
                    if (efItem == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mItem.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efItem = new Persistence.Item
                        {
                            AuditId = audit.Id
                        };
                        context.Items.Add(efItem);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efItem.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mItem.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efItem.Code = mItem.Code;
                    efItem.CompanyId = mItem.CompanyId;
                    efItem.Name = mItem.Name;
                    efItem.Description = mItem.Description;
                    efItem.HSNCode = mItem.HSNCode;
                    efItem.UnitId = mItem.UnitId;
                    efItem.IsActive = mItem.IsActive;

                    context.SaveChanges();

                    mItem.Id = efItem.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mItem;
        }

        public Domain.Item Get(int id)
        {
            var mItem = new Domain.Item();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efItem = (from i in context.Items
                                  where i.Id == id
                                  select new
                                  {
                                      Id = i.Id,
                                      Code = i.Code,
                                      CompanyId = i.CompanyId,
                                      Name = i.Name,
                                      Description = i.Description,
                                      HSNCode = i.HSNCode,
                                      UnitId = i.UnitId,
                                      IsActive = i.IsActive,
                                      AuditId = i.AuditId,
                                  }).AsEnumerable();

                    mItem = JsonSerializer.Deserialize<Domain.Item>(JsonSerializer.Serialize(efItem.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItem;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efItem = context.Items.FirstOrDefault(x => x.Id == id);
                    if (efItem != null)
                    {
                        var audit = context.AuditTrails.Find(efItem.AuditId);
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

        public List<Domain.Item> GetActiveItems()
        {
            var mItems = new List<Domain.Item>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efItems = (from i in context.Items
                                   join at in context.AuditTrails on i.AuditId equals at.Id
                                   where at.DeletedDate == null && at.DeletedBy == null && i.IsActive
                                   select new
                                   {
                                       Id = i.Id,
                                       Name = i.Name,
                                   }).AsEnumerable();

                    mItems = JsonSerializer.Deserialize<List<Domain.Item>>(JsonSerializer.Serialize(efItems.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItems;
        }
    }
}
