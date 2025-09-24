using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Infrastructure.Repository;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Application.Service
{
    public class CardService : ICardService
    {
        public readonly ICardRepository _repository;
        private readonly ILogger _logger;

        private readonly string _privateSecretKey;
        private readonly string _tokenValidationMinutes;
        private readonly PagSeguro _pagSeguro;

        public CardService(ICardRepository repository, ILogger logger, string privateSecretKey, string tokenValidationMinutes, PagSeguro pagSeguro
    )
        {
            _repository = repository;
            _logger = logger;
            _privateSecretKey = privateSecretKey;
            _tokenValidationMinutes = tokenValidationMinutes;
            _pagSeguro = pagSeguro;
    }
        public DecodedToken GetDecodeToken(string token, string secret)
        {
            DecodedToken decodedToken = new DecodedToken();
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            if (IsValidToken(token, secret))
            {
                foreach (Claim claim in jwtSecurityToken.Claims)
                {
                    if (claim.Type == "email")
                    {
                        decodedToken.email = claim.Value;
                    }
                    else if (claim.Type == "name")
                    {
                        decodedToken.name = claim.Value;
                    }
                    else if (claim.Type == "userId")
                    {
                        decodedToken.UserId = new Guid(claim.Value);
                    }
                    else if (claim.Type == "roleId")
                    {
                        decodedToken.RoleId = new Guid(claim.Value);
                    }
                }

                return decodedToken;
            }

            throw new Exception("invalidToken");
        }
        public bool IsValidToken(string token, string secret)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("emptyToken");
            }
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters();
            tokenValidationParameters.ValidateIssuer = false;
            tokenValidationParameters.ValidateAudience = false;
            tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Base64UrlEncoder.Encode(secret)));

            try
            {
                SecurityToken validatedToken;
                ClaimsPrincipal claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CardResponse CreateCardService(CreateCardRequest createCardRequest, string token)
        {
            try
            {
                var consumer = GetConsumerByConsumerId(createCardRequest.Consumer_id);
                if (consumer == null) throw new Exception("consumerExistsToConsumerId");

                var decodedToken = GetDecodeToken(token.Split(' ')[1], _privateSecretKey) ?? throw new Exception("ErrorDecodingToken");
               

                var response = TokenizationCardAsync(createCardRequest.Encrypted).Result;
                if (response[1].Contains("error_messages"))
                {
                    _logger.Error(response[1]);
                    throw new ArgumentException("cardNotSavingPagseguro", response[1]);
                }
                var cardreponse = JsonConvert.DeserializeObject<CardTokenResponse>(response[1]);

                cardreponse.Created_by = decodedToken.UserId;
                cardreponse.Consumer_id = createCardRequest.Consumer_id;
                

                var cardCreated = _repository.CreateCardRepository(cardreponse, response[0], response[1]);
                if (cardCreated == null) throw new Exception("consumerNotCreated");

                _logger.Information("[CardService - CreateCardService]: Card consumer created successfully.");
                return cardCreated;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[CardService - CreateCardService]: Error while create card consumer.");
                throw;
            }
        }

        public async Task<string[]> TokenizationCardAsync(string encrypted)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_pagSeguro.Token}");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var pagseguroRequest = new TokenCardRequest() 
                { 
                Encrypted = encrypted,
                //Holder = new HolderTokenCard()
                //{
                //    Name = card.Name
                //}
                
                };

                var body = JsonConvert.SerializeObject(pagseguroRequest);

                _logger.Information($"Request Token Card: {body}");


                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_pagSeguro.Url}tokens/cards", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                string[] result = new string[2] { body, responseContent };

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[CardService - TokenizationCardAsync]: Error when saving pagseguro card.");
                throw;
            }
        }
        public Consumer GetConsumerByConsumerId(Guid consumer_id)
        {
            try
            {
                _logger.Information("[CardService - GetConsumerByConsumerId]: Consumer retrieved successfully.");
                return _repository.GetConsumerByConsumerIdRepository(consumer_id);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public CardResponse UpdateCardService(UpdateCardRequest updateCardRequest, string token)
        {
            try
            {
                var consumer = GetConsumerByConsumerId(updateCardRequest.Consumer_id);
                if (consumer == null) throw new Exception("consumerExistsToConsumerId");

                var decodedToken = GetDecodeToken(token.Split(' ')[1], _privateSecretKey) ?? throw new Exception("ErrorDecodingToken");

               
                var response = TokenizationCardAsync(updateCardRequest.Encrypted).Result;
                if (response[1].Contains("error_messages"))
                {
                    _logger.Error(response[1]);
                    throw new ArgumentException("cardNotSavingPagseguro", response[1]);
                }
                var cardreponse = JsonConvert.DeserializeObject<CardTokenResponse>(response[1]);

                cardreponse.Created_by = decodedToken.UserId;
                cardreponse.Consumer_id = updateCardRequest.Consumer_id;
                cardreponse.Card_id = updateCardRequest.Card_id;

                var cardCreated = _repository.UpdateCardRepository(cardreponse, response[0], response[1]);
                if (cardCreated == null) throw new Exception("consumerNotUpdated");

                _logger.Information("[CardService - UpdateCardService]: Card consumer updated successfully.");
                return cardCreated;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[CardService - UpdateCardService]: Error while create card consumer.");
                throw;
            }
        }

        public List<CardResponse> GetCardService(Guid consumer_id)
        {
            try
            {
                var consumer = GetConsumerByConsumerId(consumer_id);
                if (consumer == null) throw new Exception("consumerExistsToConsumerId");

                var response = _repository.GetCardRepository(consumer_id);
                _logger.Information("[CardService - GetConsumerByConsumerId]: Card retrieved successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[CardService - CreateCardService]: Error while retrieved card consumer.");
                throw;
            }
        }

        public CardResponse DeleteCardService(Guid card_id)
        {
            try
            {

                var response = _repository.DeleteCardRepository(card_id);
                _logger.Information("[CardService - DeleteCardRepository]: Card delete successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[CardService - CreateCardService]: Error while delete card consumer.");
                throw;
            }
        }
    }
}
