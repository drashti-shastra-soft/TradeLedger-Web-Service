using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IService;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System;

namespace TradeLedgerWebService.Controllers
{
    [Authorize, RoutePrefix("api/Unit")]
    public class UnitController : ApiController
    {
        private readonly IUnitService _service;
        public UnitController(IUnitService service)
        {
            _service = service;
        }

        [HttpGet, Route("GetActiveUnit")]
        public HttpResponseMessage GetActiveUnit()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveUnit());
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
