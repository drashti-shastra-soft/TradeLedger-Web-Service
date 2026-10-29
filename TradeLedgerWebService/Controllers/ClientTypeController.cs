using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IService;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System;

namespace TradeLedgerWebService.Controllers
{
    [Authorize, RoutePrefix("api/ClientType")]
    public class ClientTypeController : ApiController
    {
        private readonly IClientTypeService _service;
        public ClientTypeController(IClientTypeService service)
        {
            _service = service;
        }

        [HttpGet, Route("GetActiveClientTypes")]
        public HttpResponseMessage GetActiveClientTypes()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveClientTypes());
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
