using Application.Service;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentOptionsService _service;
        private readonly ILogger _logger;
        public PaymentController(IPaymentOptionsService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }


        /// <summary>
        /// Endpoint responsável cadastrar meio de pagamento.
        /// </summary>
        /// <returns>Valida os dados passados e retorna os dados cadastrado.</returns>
        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Response<PaymentOptionsResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<PaymentOptionsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<PaymentOptionsResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<PaymentOptionsResponse>> CreateNewPayment([FromBody] CreatePaymentRequest createPaymentRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"];
                var response = _service.CreatePayment(createPaymentRequest, token);
                return StatusCode(StatusCodes.Status201Created, new Response<PaymentOptionsResponse>() { Status = 201, Message = $"Meio de Pagamento criado com sucesso.", Data = response, Success = true });

            }
            catch (Exception ex)
            {

                _logger.Error(ex, "Exception when creating payment method!");
                switch (ex.Message)
                {
                    case "errorWhileInsertPaymentOnDB":
                        return StatusCode(StatusCodes.Status404NotFound, new Response<PaymentOptionsResponse>() { Status = 404, Message = $"Não foi possível cadastrar meio de pagamento. Erro no processo de inserção do meio de pagamento na base de dados.", Success = false, Error = ex.Message });
                    case "errorDecodingToken":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<PaymentOptionsResponse>() { Status = 400, Message = $"Não foi possível cadastrar meio de pagamento. Erro no processo de decodificação do token.", Success = false, Error = ex.Message });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<PaymentOptionsResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }


        /// <summary>
        /// Endpoint responsável por buscar meios de pagamento.
        /// </summary>
        /// <returns>Retorna os meios de pagamento cadastrados.</returns>
        [Authorize]
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<ListPaymentOptionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ListPaymentOptionsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ListPaymentOptionsResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListPaymentOptionsResponse>> GetPaymentOptions(int? page, int? itensPerPage)
        {
            try
            {
                var filters = new Filter
                {
                    page = page ?? 1,
                    itensPerPage = itensPerPage ?? 5
                };

                var response = _service.GetPaymentOptions(filters);
                return StatusCode(StatusCodes.Status200OK, new Response<ListPaymentOptionsResponse>() { Status = 200, Message = $"Meios de pagamento retornado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when listing payment options!");
                switch (ex.Message)
                {
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ListPaymentOptionsResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por ativar/desativar meio de pagamento.
        /// </summary>
        /// <returns>Retorna produto alterado.</returns>
        [Authorize]
        [HttpPut("active")]
        [ProducesResponseType(typeof(Response<PaymentOptionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<PaymentOptionsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<PaymentOptionsResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<PaymentOptionsResponse>> UpdateOptionPayment([FromBody] UpdatePaymentRequest updatePaymentRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"];
                var response = _service.ActivePayment(updatePaymentRequest, token);
                return StatusCode(StatusCodes.Status200OK, new Response<PaymentOptionsResponse>() { Status = 200, Message = response.Active ? $"Pagamento ativado com sucesso." : $"Pagamento desativado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while upting payment!");
                switch (ex.Message)
                {
                    case "errorWhileUpdatedtPaymentOnDB":
                        return StatusCode(StatusCodes.Status404NotFound, new Response<PaymentOptionsResponse>() { Status = 404, Message = $"Não foi possível ativar/desativar. Erro no processo de alteração do meio de pagamento na base de dados.", Success = false, Error = ex.Message });
                    case "errorDecodingToken":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<PaymentOptionsResponse>() { Status = 400, Message = $"Não foi possível ativar/desativar. Erro no processo de decodificação do token.", Success = false, Error = ex.Message });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<PaymentOptionsResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por buscar valor minimo aceito no pedido.
        /// </summary>
        /// <returns>Retorna o valor correspondente de cada parceiro.</returns>
        [Authorize]
        [HttpGet("value-minimum")]
        [ProducesResponseType(typeof(Response<ValueMinimun>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ValueMinimun>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ValueMinimun>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ValueMinimun>> GetPagSeguroValueMin(Guid partner_id)
        {
            try
            {

                var response = _service.GetPagSeguroValueMin(partner_id);
                return StatusCode(StatusCodes.Status200OK, new Response<ValueMinimun>() { Status = 200, Message = $"Dados retornado com sucesso!", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when retrieved value minimum!");
                switch (ex.Message)
                {
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ValueMinimun>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por Obter access token pagseguro.
        /// </summary>
        /// <returns>Retorna access token da Pagseguro.</returns>
        [AllowAnonymous]
        [HttpGet("access-token")]
        [ProducesResponseType(typeof(Response<AccessTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<AccessTokenResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<AccessTokenResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<AccessTokenResponse>> AccessTokenPagSeguro(string code, string state)
        {
            try
            {
                var response = _service.PostAccessTokenPagSeguro(code, state);
                return RedirectPermanent(response.Redirect_url_pam_pos_oauth_pagseguro);
               // return StatusCode(StatusCodes.Status201Created, new Response<AccessTokenResponse>() { Status = 201, Message = $"Access token retornado com sucesso!", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when created access token pagseguro!");
                switch (ex.Message)
                {
                    case "errorAccessToken":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<AccessTokenResponse>() { Status = 400, Message = $"Não foi possível buscar access token. Erro no processo de obter access token pagseguro.", Success = false, Error = ex.Message });
                    case "errorDecodingToken":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<AccessTokenResponse>() { Status = 400, Message = $"Não foi possível buscar access token. Erro no processo de decodificação do token.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<AccessTokenResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }
    }
}
