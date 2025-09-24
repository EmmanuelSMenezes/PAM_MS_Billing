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
using Serilog;

namespace Application.Service
{
    public class DeliveryOptionsService : IDeliveryOptionsService
    {
        private readonly IDeliveryOptionsRepository _repository;
        private readonly ILogger _logger;
        private readonly string _privateSecretKey;
        private readonly string _tokenValidationMinutes;

        public DeliveryOptionsService(
          IDeliveryOptionsRepository repository,
          ILogger logger,
          string privateSecretKey,
          string tokenValidationMinutes
          )
        {
            _repository = repository;
            _logger = logger;
            _privateSecretKey = privateSecretKey;
            _tokenValidationMinutes = tokenValidationMinutes;
        }

        

        public DeliveryOptionsResponse CreateDelivery(CreateDeliveryRequest createDeliveryRequest, string token)
        {
            try
            {
                var decodedToken = GetDecodeToken(token.Split(' ')[1], _privateSecretKey);
                if (decodedToken == null) throw new Exception("errorDecodingToken");

                createDeliveryRequest.Created_by = decodedToken.UserId;
                var create = _repository.CreateDelivery(createDeliveryRequest);

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

        public ListDeliveryOptionsResponse GetDeliveryOptions(Filter filter)
        {

            try
            {
                var getAll = _repository.GetDeliveryOptions(filter);

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

        public DeliveryOptionsResponse ActiveDelivery(UpdateDeliveryRequest updateDeliveryRequest, string token)
        {

            try
            {
                var decodedToken = GetDecodeToken(token.Split(' ')[1], _privateSecretKey);
                if (decodedToken == null) throw new Exception("errorDecodingToken");
                updateDeliveryRequest.Updated_by = decodedToken.UserId;

                var update = _repository.ActiveDelivery(updateDeliveryRequest);

                if (update == null) throw new Exception("");
                return update;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
