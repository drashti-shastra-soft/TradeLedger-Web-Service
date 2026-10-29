using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IService;
using System.Net.Http;
using System.Web.Http;
using System.Net;   
using System;

namespace TradeLedgerWebService.Controllers
{
    [Authorize, RoutePrefix("api/Client")]
    public class ClientController : ApiController
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }

        [HttpPost, Route("GetAll")]
        public HttpResponseMessage GetAll(Domain.ClientLister mLister)
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
        public HttpResponseMessage Upsert(Domain.Client mClient)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Upsert(mClient));
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


        [HttpGet, Route("GetActiveClient")]
        public HttpResponseMessage GetActiveClient()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveClient());
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
                _service.UpdateAddress(id, loginUserId,addressId);
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

        [HttpGet, Route("GetActiveSellerClient")]
        public HttpResponseMessage GetActiveSellerClient()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveSellerClient());
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

        [HttpGet, Route("GetActiveBuyerClient")]
        public HttpResponseMessage GetActiveBuyerClient()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveBuyerClient());
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
