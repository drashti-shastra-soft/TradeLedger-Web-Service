using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class AddressRepository : IAddressRepository
    {
        public Domain.AddressLister GetAll(Domain.AddressLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from a in context.Addresses
                                join at in context.AuditTrails on a.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = a.Id,
                                    Line1 = a.Line1,
                                    Line2 = a.Line2,
                                    Line3 = a.Line3,
                                    City = a.City,
                                    State = a.State,
                                    ZipCode = a.ZipCode,
                                    AuditId = a.AuditId,
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Line1.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Line1) && x.Line1.ToUpper().Contains(criteria.Line1.ToUpper()));

                        if (criteria.Line1.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Line1) && x.Line1.ToUpper().Contains(criteria.Line1.ToUpper()));

                    }
                    // Switch to in-memory for complex string filters
                    var efClient = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efClient.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efClient = efClient.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Address>>(JsonSerializer.Serialize(efClient.ToList()));

                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.Address Upsert(Domain.Address mAddress)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efAddress = context.Addresses.FirstOrDefault(x => x.Id == mAddress.Id);
                    if (efAddress == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mAddress.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efAddress = new Persistence.Address
                        {
                            AuditId = audit.Id
                        };
                        context.Addresses.Add(efAddress);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efAddress.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mAddress.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efAddress.Line1 = mAddress.Line1;
                    efAddress.Line2 = mAddress.Line2;
                    efAddress.Line3 = mAddress.Line3;
                    efAddress.City = mAddress.City;
                    efAddress.State = mAddress.State;
                    efAddress.ZipCode = mAddress.ZipCode;
                    context.SaveChanges();

                    mAddress.Id = efAddress.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mAddress;
        }

        public Domain.Address Get(int id)
        {
            var mAddress = new Domain.Address();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efAddress = (from a in context.Addresses
                                     join c in context.Clients on a.Id equals c.AddressId
                                     join b in context.Brokers on a.Id equals b.AddressId
                                     where c.Id == id && b.Id == id
                                     select new
                                     {
                                         Id = a.Id,
                                         Line1 = a.Line1,
                                         Line2 = a.Line2,
                                         Line3 = a.Line3,
                                         City = a.City,
                                         State = a.State,
                                         ZipCode = a.ZipCode,
                                         AuditId = a.AuditId,
                                     }).AsEnumerable();

                    mAddress = JsonSerializer.Deserialize<Domain.Address>(JsonSerializer.Serialize(efAddress.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mAddress;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efAddress = context.Addresses.FirstOrDefault(x => x.Id == id);
                    if (efAddress != null)
                    {
                        var audit = context.AuditTrails.Find(efAddress.AuditId);
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
