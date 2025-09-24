using Application.Service;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{
    [Route("card")]
    [Authorize]
    [ApiController]
    public class CardController : Controller
    {
        private readonly ICardService _service;
        private readonly ILogger _logger;

        public CardController(ICardService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint responsável por validar e armazenar um cartão no PagBank.
        /// </summary>
        /// <returns>Valida os dados passados e retorna o cartão cadastrado.</returns>

        [HttpPost("create")]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<CardResponse>> CreateConsumer([FromBody] CreateCardRequest createCardRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"];
                var response = _service.CreateCardService(createCardRequest, token);
                return StatusCode(StatusCodes.Status201Created, new Response<CardResponse>() { Status = 200, Message = $"Cartão do consumidor registrado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while creating new card!");
                switch (ex.Message)
                {
                    case "consumerExistsToConsumerId":
                        return StatusCode(StatusCodes.Status403Forbidden, new Response<CardResponse>() { Status = 403, Message = $"Consumidor não localizado!", Success = false });
                    case "ErrorDecodingToken":
                        return StatusCode(StatusCodes.Status403Forbidden, new Response<CardResponse>() { Status = 403, Message = $"Não foi possível registrar cartão. Erro no processo de decodificação do token!", Success = false });
                    case string message when message.Contains("cardNotSavingPagseguro"):
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<CardResponse>() { Status = 400, Message = $"Não foi possível registrar cartão. Erro no processo de validação no PagBank!", Success = false, Error = ex.Message }); ;
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<CardResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por alterar cartão do consumidor.
        /// </summary>
        /// <returns>Valida os dados passados para alteração e retorna o cartão alterado.</returns>

        [HttpPut("update")]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<CardResponse>> UpdateConsumer([FromBody] UpdateCardRequest updateCardRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"];
                var response = _service.UpdateCardService(updateCardRequest, token);
                return StatusCode(StatusCodes.Status200OK, new Response<CardResponse>() { Status = 200, Message = $"Cartão do consumidor alterado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while updating card!");
                switch (ex.Message)
                {
                    case "consumerExistsToConsumerId":
                        return StatusCode(StatusCodes.Status403Forbidden, new Response<CardResponse>() { Status = 403, Message = $"Consumidor não localizado!", Success = false, Error = ex.Message });
                    case "ErrorDecodingToken":
                        return StatusCode(StatusCodes.Status403Forbidden, new Response<CardResponse>() { Status = 403, Message = $"Não foi possível alterar cartão. Erro no processo de decodificação do token!", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<CardResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por buscar cartão do consumidor.
        /// </summary>
        /// <returns>Valida os dados passados e retorna o cartão do consumidor.</returns>

        [HttpGet("get/consumer/{consumer_id}")]
        [ProducesResponseType(typeof(Response<List<CardResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<List<CardResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<List<CardResponse>>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Response<List<CardResponse>>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<List<CardResponse>>> GetCardByConsumerId([Required(ErrorMessage = "Informe o id do consumidor")] Guid consumer_id)
        {
            try
            {
                var response = _service.GetCardService(consumer_id);
                return StatusCode(StatusCodes.Status200OK, new Response<List<CardResponse>>() { Status = 200, Message = $"Cartão do consumidor retornado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while updating card!");
                switch (ex.Message)
                {
                    case "consumerExistsToConsumerId":
                        return StatusCode(StatusCodes.Status403Forbidden, new Response<List<CardResponse>>() { Status = 403, Message = $"Consumidor não localizado!", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<List<CardResponse>>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por buscar cartão do consumidor.
        /// </summary>
        /// <returns>Valida os dados passados e retorna o cartão do consumidor.</returns>

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Response<CardResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<CardResponse>> DeleteCard([Required(ErrorMessage = "Informe o id do cartão")] Guid card_id)
        {
            try
            {
                var response = _service.DeleteCardService(card_id);
                return StatusCode(StatusCodes.Status200OK, new Response<CardResponse>() { Status = 200, Message = $"Cartão do consumidor removido com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while updating card!");
                switch (ex.Message)
                {
                    case "consumerExistsToConsumerId":
                        return StatusCode(StatusCodes.Status403Forbidden, new Response<CardResponse>() { Status = 403, Message = $"Consumidor não localizado!", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<CardResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false, Error = ex.Message });
                }
            }
        }
    }
}
