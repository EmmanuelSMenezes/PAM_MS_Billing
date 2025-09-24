using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Application.Service
{
    public class PaymentOptionsService : IPaymentOptionsService
    {
        private readonly IPaymentRepository _repository;
        private readonly ILogger _logger;
        private readonly string _privateSecretKey;
        private readonly string _tokenValidationMinutes;
        private PagSeguro _pagSeguroAccess = new PagSeguro();
        public PaymentOptionsService(
          IPaymentRepository repository,
          ILogger logger,
          string privateSecretKey,
          string tokenValidationMinutes,
          PagSeguro pagSeguroAccess
          )
        {
            _repository = repository;
            _logger = logger;
            _privateSecretKey = privateSecretKey;
            _tokenValidationMinutes = tokenValidationMinutes;
            _pagSeguroAccess = pagSeguroAccess;
        }

        public PaymentOptionsResponse ActivePayment(UpdatePaymentRequest updatePaymentRequest, string token)
        {

            try
            {
                var decodedToken = GetDecodeToken(token.Split(' ')[1], _privateSecretKey);
                if (decodedToken == null) throw new Exception("errorDecodingToken");
                updatePaymentRequest.Updated_by = decodedToken.UserId;

                var update = _repository.ActivePayment(updatePaymentRequest);

                if (update == null) throw new Exception("");
                return update;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public PaymentOptionsResponse CreatePayment(CreatePaymentRequest createPaymentRequest, string token)
        {
            try
            {
                var decodedToken = GetDecodeToken(token.Split(' ')[1], _privateSecretKey);
                if (decodedToken == null) throw new Exception("errorDecodingToken");

                createPaymentRequest.Created_by = decodedToken.UserId;
                var create = _repository.CreatePayment(createPaymentRequest);

                if (create == null) throw new Exception("");

                return create;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public ValueMinimun GetPagSeguroValueMin(Guid partner_id)
        {
            try
            {
                var get = _repository.GetPagSeguroValueMin(partner_id);

                return get;


            }
            catch (Exception)
            {
                throw;
            }
        }

        public ListPaymentOptionsResponse GetPaymentOptions(Filter filter)
        {

            try
            {
                var getAll = _repository.GetPaymentOptions(filter);

                switch (getAll)
                {

                    case var _ when getAll == null:
                        throw new Exception("");
                    default: return getAll;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public AccessTokenResponse PostAccessTokenPagSeguro(string code, string state)
        {
            try
            {
                var response = AccessToken(code, state).Result;

                //var decodedToken = GetDecodeToken(state.Split(' ')[1], _privateSecretKey);
                //if (decodedToken == null) throw new Exception("errorDecodingToken");

                var partner = _repository.GetPartnerByUserId(Guid.Parse(state));
                _repository.CreateAccessTokenHistory(partner, response);

                if (response.Contains("error_messages"))
                {
                    throw new Exception("errorAccessToken");
                }
                var accesstoken = JsonConvert.DeserializeObject<AccessTokenResponse>(response);

                if (accesstoken != null)
                {
                    var bank = _repository.CreateBank(accesstoken.Account_id, partner);
                    if (bank != null)
                    {
                        accesstoken.Redirect_url_pam_pos_oauth_pagseguro = _pagSeguroAccess.Redirect_url_pam_pos_oauth_pagseguro;
                        return accesstoken;
                    }
                    
                }
                throw new Exception();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> AccessToken(string code, string state)
        {
            try
            {
                var accesstoken = new AccessToken()
                {
                    grant_type = "authorization_code",
                    code = code,
                    sms_code = code,
                    redirect_uri = _pagSeguroAccess.Redirect_uri
                };
                var content = JsonConvert.SerializeObject(accesstoken); ;
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_pagSeguroAccess.Url}oauth2/token"),
                    Headers =
                            {
                                { "accept", "application/json" },
                                { "Authorization", $"Bearer {_pagSeguroAccess.Token}" },
                                { "X_CLIENT_ID", $"{_pagSeguroAccess.Client_id}" },
                                { "X_CLIENT_SECRET", $"{_pagSeguroAccess.Client_secret}" },
                            },
                    Content = new StringContent(content)
                    {
                        Headers =
                            {
                                ContentType = new MediaTypeHeaderValue("application/json")
                            }
                    }
                };
                using (var response = await client.SendAsync(request))
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.Information($"Request URL Access Token : {request.RequestUri}");
                    _logger.Information($"Request Access Token : {content}");
                    return body;
                }
                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
