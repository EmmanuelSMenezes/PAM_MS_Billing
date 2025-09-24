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
    [Route("delivery")]
    [ApiController]
    [Authorize]
    public class DeliveryController : Controller
    {
        private readonly IDeliveryOptionsService _service;
        private readonly ILogger _logger;
        public DeliveryController(IDeliveryOptionsService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }


        /// <summary>
        /// Endpoint responsável cadastrar tipo de frete.
        /// </summary>
        /// <returns>Valida os dados passados e retorna os dados cadastrado.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Response<DeliveryOptionsResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<DeliveryOptionsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<DeliveryOptionsResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<DeliveryOptionsResponse>> CreateNewTypeFreight([FromBody] CreateDeliveryRequest createDeliveryRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"];
                var response = _service.CreateDelivery(createDeliveryRequest, token);
                return StatusCode(StatusCodes.Status201Created, new Response<DeliveryOptionsResponse>() { Status = 201, Message = $"Meio de Pagamento criado com sucesso.", Data = response, Success = true });

            }
            catch (Exception ex)
            {

                _logger.Error(ex, "Exception when creating delivery option!");
                switch (ex.Message)
                {
                    case "errorWhileInsertFreightOnDB":
                        return StatusCode(StatusCodes.Status404NotFound, new Response<DeliveryOptionsResponse>() { Status = 404, Message = $"Não foi possível cadastrar opção de entrega. Erro no processo de inserção.", Success = false, Error = ex.Message });
                    case "errorDecodingToken":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<DeliveryOptionsResponse>() { Status = 400, Message = $"Não foi possível cadastrar meio de pagamento. Erro no processo de decodificação do token.", Success = false, Error = ex.Message });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<DeliveryOptionsResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }   
        }


        /// <summary>
        /// Endpoint responsável por buscar meios de entrega.
        /// </summary>
        /// <returns>Retorna os meios de entrega cadastrados.</returns>
        [Authorize]
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<ListDeliveryOptionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ListDeliveryOptionsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ListDeliveryOptionsResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListDeliveryOptionsResponse>> GetDeliveryOptions(int? page, int? itensPerPage)
        {
            try
            {
                var filters = new Filter
                {
                    page = page ?? 1,
                    itensPerPage = itensPerPage ?? 5
                };

                var response = _service.GetDeliveryOptions(filters);
                return StatusCode(StatusCodes.Status200OK, new Response<ListDeliveryOptionsResponse>() { Status = 200, Message = $"Meios de entrega retornado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when listing delivery options!");
                switch (ex.Message)
                {
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ListDeliveryOptionsResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por ativar/desativar opção de entrega.
        /// </summary>
        /// <returns>Retorna Opção de entrega alterado.</returns>
        [Authorize]
        [HttpPut("active")]
        [ProducesResponseType(typeof(Response<DeliveryOptionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<DeliveryOptionsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<DeliveryOptionsResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<DeliveryOptionsResponse>> UpdateDeliveryOptions([FromBody] UpdateDeliveryRequest updateDeliveryRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"];
                var response = _service.ActiveDelivery(updateDeliveryRequest, token);
                return StatusCode(StatusCodes.Status200OK, new Response<DeliveryOptionsResponse>() { Status = 200, Message = response.Active ? $"Opção de entrega ativado com sucesso." : $"Opção de entrega desativado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while upting option delivery!");
                switch (ex.Message)
                {
                    case "errorWhileUpdatedtDeliveryOnDB":
                        return StatusCode(StatusCodes.Status404NotFound, new Response<DeliveryOptionsResponse>() { Status = 404, Message = $"Não foi possível ativar/desativar. Erro no processo de alteração na base de dados.", Success = false, Error = ex.Message });
                    case "errorDecodingToken":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<DeliveryOptionsResponse>() { Status = 400, Message = $"Não foi possível ativar/desativar. Erro no processo de decodificação do token.", Success = false, Error = ex.Message });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<DeliveryOptionsResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

    }
}
