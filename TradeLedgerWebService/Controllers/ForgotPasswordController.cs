using TradeLedgerWebService.Infrastructure.CustomException;
using TradeLedgerWebService.Infrastructure.IService;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System;

namespace TradeLedgerWebService.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        #region Declarations
        private readonly IForgotPasswordService _service;

        public ForgotPasswordController(IForgotPasswordService service)
        {
            _service = service;
        }
        #endregion

        [HttpPut]
        public HttpResponseMessage Initiate(Domain.ForgotPassword mForgotPassword)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.Initiate(mForgotPassword);
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (APIRequestFailedException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
