using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;
 
namespace TradeLedgerWebService.Repository
{
    public class UserRepository : IUserRepository
    {
        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var query = from u in context.Users
                                join c in context.Companies on u.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join role in context.Roles on u.RoleId equals role.Id
                                join at in context.AuditTrails on u.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null        
                                select new
                                {
                                    Id = u.Id,
                                    CompanyId = u.CompanyId,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    EmailAddress = u.EmailAddress,
                                    MobileNumber = u.MobileNumber,
                                    RoleId = u.RoleId,
                                    Password = u.Password,
                                    IsActive = u.IsActive,
                                    Role = role.Name,
                                    Company = c.Name,
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.FirstName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()));

                        if (criteria.LastName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.ToUpper().Contains(criteria.LastName.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

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
                    var efUsers = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efUsers.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efUsers = efUsers.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.User>>(JsonSerializer.Serialize(efUsers.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.User Upsert(Domain.User mUser)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == mUser.Id);

                    if (efUser == null)
                    {
                        var audit = new Persistence.AuditTrail
                        {
                            CreatedBy = mUser.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efUser = new Persistence.User
                        {
                            Password = mUser.Password,
                            AuditId = audit.Id
                        };

                        context.Users.Add(efUser);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efUser.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mUser.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }
                    efUser.FirstName = mUser.FirstName;
                    efUser.LastName = mUser.LastName;
                    efUser.EmailAddress = mUser.EmailAddress;
                    efUser.MobileNumber = mUser.MobileNumber;
                    efUser.RoleId = mUser.RoleId;
                    efUser.CompanyId = mUser.CompanyId;
                    efUser.IsActive = mUser.IsActive;

                    context.SaveChanges();  
                    mUser.Id = efUser.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public Domain.User Get(int id)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efUser = (from u in context.Users
                                  where u.Id == id
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      EmailAddress = u.EmailAddress,
                                      MobileNumber = u.MobileNumber,
                                      RoleId = u.RoleId,
                                      IsActive = u.IsActive,
                                      CompanyId = u.CompanyId,
                                      AuditId = u.AuditId
                                  }).FirstOrDefault();

                    if (efUser != null)
                    {
                        mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == id);
                    if (efUser != null)
                    {
                        var audit = context.AuditTrails.Find(efUser.AuditId);
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

        public Domain.User GetBy(string emailAddress)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efUser = (from u in context.Users
                                  where u.EmailAddress.Trim().ToUpper() == emailAddress.Trim().ToUpper()
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      EmailAddress = u.EmailAddress,
                                      Password = u.Password,
                                      MobileNumber = u.MobileNumber,
                                      RoleId = u.RoleId,
                                      IsActive = u.IsActive,
                                      CompanyId = u.CompanyId
                                  }).AsEnumerable();

                    mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public string GetPasswordBy(int id) 
        { 
            string password = string.Empty;
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    password = context.Users.Where(x => x.Id == id).Select(s => s.Password).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return password;
        }

        public string SetPasswordBy(int id, string password)
        {
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efUser = context.Users.Where(x => x.Id == id).FirstOrDefault();
                    if (efUser != null && efUser.Id > default(int))
                    {
                        efUser.Password = password;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return password;
        }
    }
}
