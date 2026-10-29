using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Collections.Generic;   
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class ClientRepository : IClientRepository
    {
        public Domain.ClientLister GetAll(Domain.ClientLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from cl in context.Clients
                                join c in context.Companies on cl.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join ct in context.ClientTypes on cl.ClientTypeId equals ct.Id
                                join at in context.AuditTrails on cl.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = cl.Id,
                                    ClientTypeId = cl.ClientTypeId,
                                    CompanyId = cl.CompanyId,
                                    ClientType = ct.Name,
                                    Name = cl.Name,
                                    ContactPerson = cl.ContactPerson,
                                    Brand = cl.Brand,
                                    EmailAddress = cl.EmailAddress,
                                    MobileNumber = cl.MobileNumber,
                                    PanNumber = cl.PanNumber,
                                    GSTNumber = cl.GSTNumber,
                                    AuditId = cl.AuditId,
                                    IsActive = cl.IsActive,
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


                        if (criteria.Brand.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Brand) && x.Brand.ToUpper().Contains(criteria.Brand.ToUpper()));

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.GSTNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.GSTNumber) && x.GSTNumber.ToUpper().Contains(criteria.GSTNumber.ToUpper()));

                        if (criteria.PanNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.PanNumber) && x.PanNumber.ToUpper().Contains(criteria.PanNumber.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.ContactPerson.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.ContactPerson) && x.ContactPerson.ToUpper().Contains(criteria.ContactPerson.ToUpper()));
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
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Client>>(JsonSerializer.Serialize(efClient.ToList()));

                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.Client Upsert(Domain.Client mClient)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClient = context.Clients.FirstOrDefault(x => x.Id == mClient.Id);
                    if (efClient == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mClient.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efClient = new Persistence.Client
                        {
                            AddressId = mClient.AddressId,
                            AuditId = audit.Id
                        };
                        context.Clients.Add(efClient);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efClient.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mClient.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }
               
                    efClient.ClientTypeId = mClient.ClientTypeId;
                    efClient.CompanyId = mClient.CompanyId;
                    efClient.Brand = mClient.Brand;
                    efClient.Name = mClient.Name;
                    efClient.ContactPerson = mClient.ContactPerson;
                    efClient.EmailAddress = mClient.EmailAddress;
                    efClient.MobileNumber = mClient.MobileNumber;
                    efClient.GSTNumber = mClient.GSTNumber;
                    efClient.PanNumber = mClient.PanNumber;
                    efClient.IsActive = mClient.IsActive;
                    context.SaveChanges();

                    mClient.Id = efClient.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mClient;
        }

        public Domain.Client Get(int id)
        {
            var mClient = new Domain.Client();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClient = (from cl in context.Clients
                                     where cl.Id == id
                                     select new
                                     {
                                         Id = cl.Id,
                                         Name = cl.Name,
                                         ClientTypeId = cl.ClientTypeId,
                                         CompanyId = cl.CompanyId,
                                         Brand = cl.Brand,
                                         ContactPerson = cl.ContactPerson,
                                         EmailAddress = cl.EmailAddress,
                                         MobileNumber = cl.MobileNumber,
                                         PanNumber = cl.PanNumber,
                                         GSTNumber = cl.GSTNumber,
                                         IsActive = cl.IsActive,
                                         AuditId = cl.AuditId
                                     }).AsEnumerable();

                    mClient = JsonSerializer.Deserialize<Domain.Client>(JsonSerializer.Serialize(efClient.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mClient;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClient = context.Clients.FirstOrDefault(x => x.Id == id);
                    if (efClient != null)
                    {
                        var audit = context.AuditTrails.Find(efClient.AuditId);
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
                    var efClient = context.Clients.FirstOrDefault(x => x.Id == id);
                    if (efClient != null)
                    {
                        efClient.AddressId = addressId;

                        var audit = context.AuditTrails.Find(efClient.AuditId);
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

        public List<Domain.Client> GetActiveClient()
        {
            var mClient = new List<Domain.Client>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClient = (from c in context.Clients
                                    join at in context.AuditTrails on c.AuditId equals at.Id
                                    where at.DeletedDate == null && at.DeletedBy == null && c.IsActive
                                    select new
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                    }).AsEnumerable();

                    mClient = JsonSerializer.Deserialize<List<Domain.Client>>(JsonSerializer.Serialize(efClient.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mClient;
        }

        public List<Domain.Client> GetActiveSellerClient()
        {
            var mClient = new List<Domain.Client>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClient = (from c in context.Clients
                                    join at in context.AuditTrails on c.AuditId equals at.Id
                                    where at.DeletedDate == null && at.DeletedBy == null && c.IsActive && c.ClientTypeId == 1
                                    //where c.IsActive && c.ClientTypeId == 1
                                    select new
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                    }).AsEnumerable();

                    mClient = JsonSerializer.Deserialize<List<Domain.Client>>(JsonSerializer.Serialize(efClient.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mClient;
        }

        public List<Domain.Client> GetActiveBuyerClient()
        {
            var mClient = new List<Domain.Client>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClient = (from c in context.Clients
                                    join at in context.AuditTrails on c.AuditId equals at.Id
                                    where at.DeletedDate == null && at.DeletedBy == null && c.IsActive && c.ClientTypeId == 2
                                    //where c.IsActive && c.ClientTypeId == 2
                                    select new
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                    }).AsEnumerable();

                    mClient = JsonSerializer.Deserialize<List<Domain.Client>>(JsonSerializer.Serialize(efClient.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mClient;
        }

    }
}
