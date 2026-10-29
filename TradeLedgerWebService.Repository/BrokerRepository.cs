using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class BrokerRepository : IBrokerRepository
    {
        public Domain.BrokerLister GetAll(Domain.BrokerLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from b in context.Brokers
                                join c in context.Companies on b.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join at in context.AuditTrails on b.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = b.Id,
                                    CompanyId = b.CompanyId,
                                    Name = b.Name,
                                    ContactPerson = b.ContactPerson,
                                    EmailAddress = b.EmailAddress,
                                    PhoneNumber = b.PhoneNumber,
                                    PANNumber = b.PANNumber,
                                    AadharNumber = b.AadharNumber,
                                    Brokerage = b.Brokerage,
                                    AddressId = b.AddressId,
                                    IsActive = b.IsActive,
                                    Company = c.Name,
                                };

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.ContactPerson.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.ContactPerson) && x.ContactPerson.ToUpper().Contains(criteria.ContactPerson.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.PhoneNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToUpper().Contains(criteria.PhoneNumber.ToUpper()));

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }

                    // Switch to in-memory for complex string filters
                    var efBroker = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efBroker.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efBroker = efBroker.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Broker>>(JsonSerializer.Serialize(efBroker.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Broker Upsert(Domain.Broker mBrokers)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efBrokers = context.Brokers.FirstOrDefault(x => x.Id == mBrokers.Id);
                    if (efBrokers == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mBrokers.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efBrokers = new Persistence.Broker
                        {
                            AddressId = mBrokers.AddressId,
                            AuditId = audit.Id
                        };
                        context.Brokers.Add(efBrokers);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efBrokers.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mBrokers.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efBrokers.Name = mBrokers.Name;
                    efBrokers.CompanyId = mBrokers.CompanyId;
                    efBrokers.ContactPerson = mBrokers.ContactPerson;
                    efBrokers.EmailAddress = mBrokers.EmailAddress;
                    efBrokers.PhoneNumber = mBrokers.PhoneNumber;
                    efBrokers.PANNumber = mBrokers.PANNumber;
                    efBrokers.AadharNumber = mBrokers.AadharNumber;
                    efBrokers.Brokerage = mBrokers.Brokerage;
                    efBrokers.IsActive = mBrokers.IsActive;
                    context.SaveChanges();
                    mBrokers.Id = efBrokers.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mBrokers;
        }

        public Domain.Broker Get(int id)
        {
            var mBroker = new Domain.Broker();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efBroker = (from b in context.Brokers
                                    where b.Id == id
                                    select new
                                    {
                                        Id = b.Id,
                                        Name = b.Name,
                                        ContactPerson = b.ContactPerson,
                                        CompanyId = b.CompanyId,
                                        EmailAddress = b.EmailAddress,
                                        PhoneNumber = b.PhoneNumber,
                                        PANNumber = b.PANNumber,
                                        AadharNumber = b.AadharNumber,
                                        Brokerage = b.Brokerage,
                                        AddressId = b.AddressId,
                                        IsActive = b.IsActive,
                                    }).FirstOrDefault();

                    if (efBroker != null)
                    {
                        mBroker = JsonSerializer.Deserialize<Domain.Broker>(JsonSerializer.Serialize(efBroker));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mBroker;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efBroker = context.Brokers.FirstOrDefault(x => x.Id == id);
                    if (efBroker != null)
                    {
                        var audit = context.AuditTrails.Find(efBroker.AuditId);
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

        public void UpdateAddress(int id, int addressId, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efBroker = context.Brokers.FirstOrDefault(x => x.Id == id);
                    if (efBroker != null)
                    {
                        efBroker.AddressId = addressId;

                        var audit = context.AuditTrails.Find(efBroker.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = loginUserId;
                            audit.LastModifiedDate = DateTime.Now;
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

        public List<Domain.Broker> GetActiveBroker()
        {
            var mBroker = new List<Domain.Broker>();
            {
                try
                {
                    using (var context = new Persistence.TradeLedgerDB())
                    {
                        var efBroker = (from b in context.Brokers
                                        join at in context.AuditTrails on b.AuditId equals at.Id
                                        where at.DeletedDate == null && at.DeletedBy == null && b.IsActive
                                        select new
                                        {
                                            Id = b.Id,
                                            Name = b.Name,
                                        }).AsEnumerable();

                        mBroker = JsonSerializer.Deserialize<List<Domain.Broker>>(JsonSerializer.Serialize(efBroker.ToList()));
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return mBroker;
            }
        }


    }

}
