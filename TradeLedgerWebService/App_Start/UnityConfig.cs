using System.Web.Http;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using TradeLedgerWebService.Repository;
using TradeLedgerWebService.Service;
using Unity;
using Unity.WebApi;

namespace TradeLedgerWebService
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            //Service
            container.RegisterType<IAddressService, AddressService>();
            container.RegisterType<IBrokerService, BrokerService>();
            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IChangePasswordService, ChangePasswordService>();
            container.RegisterType<IClientService, ClientService>();
            container.RegisterType<IClientTypeService, ClientTypeService>();
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IContractItemService, ContractItemService>();
            container.RegisterType<IContractService, ContractService>();
            container.RegisterType<IItemService, ItemService>();
            container.RegisterType<IFileReaderService, FileReaderService>();
            container.RegisterType<IForgotPasswordService, ForgotPasswordService>();
            container.RegisterType<IJWTokenService, JWTokenService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IEmailSenderService, EmailSenderService>();
            container.RegisterType<IUnitService, UnitService>();
            container.RegisterType<IUserService, UserService>();

            //Repository
            container.RegisterType<IAddressRepository, AddressRepository>();
            container.RegisterType<IBrokerRepository, BrokerRepository>();
            container.RegisterType<IClientRepository, ClientRepository>();
            container.RegisterType<IClientTypeRepository, ClientTypeRepository>();
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<IContractItemRepository, ContractItemRepository>();
            container.RegisterType<IContractRepository, ContractRepository>();
            container.RegisterType<IItemRepository, ItemRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();
            container.RegisterType<IUnitRepository, UnitRepository>();
            container.RegisterType<IUserRepository, UserRepository>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}