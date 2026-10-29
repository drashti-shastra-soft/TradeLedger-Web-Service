using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IService;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System;

namespace TradeLedgerWebService.Controllers
{
    [Authorize, RoutePrefix("api/Broker")]
    public class BrokerController : ApiController
    {
        private readonly IBrokerService _service;

        public BrokerController(IBrokerService service)
        {
            _service = service;
        }

        [HttpPost, Route("GetAll")]
        public HttpResponseMessage GetAll(Domain.BrokerLister mLister)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetAll(mLister));
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

        [HttpPost]
        public HttpResponseMessage Upsert(Domain.Broker mBroker)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Upsert(mBroker));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unexpected Error: " + ex.Message);
            }
            return response;
        }

        [HttpGet, Route("{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Get(id));
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

        [HttpDelete, Route("{id:int}/LoginUser/{loginUserId:int}")]
        public HttpResponseMessage Delete(int id, int loginUserId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.Delete(id, loginUserId);
                response = Request.CreateResponse(HttpStatusCode.NoContent);
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

        [HttpPatch, Route("{id:int}/AddressId/{addressId:int}/LoginUser/{loginUserId:int}")]
        public HttpResponseMessage UpdateAddress(int id, int addressId, int loginUserId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.UpdateAddress(id, loginUserId, addressId);
                response = Request.CreateResponse(HttpStatusCode.NoContent);
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

        [HttpGet, Route("GetActiveBroker")]
        public HttpResponseMessage GetActiveBroker()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveBroker());
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
