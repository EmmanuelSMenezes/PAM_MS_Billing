using Application.Service;
using Domain.Model;
using FluentValidation.AspNetCore;
using FluentValidation;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using System.IO;
using System.Linq;
using System.Text;
using WebApi.Extensions;

namespace MS_Billing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddControllers().AddFluentValidation();
            services.AddControllers();
            services.AddCors();
            services.AddLogging();
            services.AddWorker(Configuration);
            services.AddMSBilling(Configuration);

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MSBillingSettings").GetSection("PrivateSecretKey").Value);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Add framework services.

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "V1";
                    document.Info.Title = "PAM - Microservice Billing";
                    document.Info.Description = "API's Documentation of Microservice Billing of PAM Plataform";
                };

                config.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                });

                config.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            string logFilePath = Configuration.GetSection("LogSettings").GetSection("LogFilePath").Value;
            string logFileName = Configuration.GetSection("LogSettings").GetSection("LogFileName").Value;

            string connectionString = Configuration.GetSection("MSBillingSettings").GetSection("ConnectionString").Value;
            string privateSecretKey = Configuration.GetSection("MSBillingSettings").GetSection("PrivateSecretKey").Value;
            string tokenValidationMinutes = Configuration.GetSection("MSBillingSettings").GetSection("TokenValidationMinutes").Value;
            string otpValidationMinutes = Configuration.GetSection("MSBillingSettings").GetSection("OtpValidationMinutes").Value;

            HttpEndPoints httpEndPoints = new HttpEndPoints()
            {
                MSOrderBaseUrl = Configuration.GetSection("HttpEndPoints").GetSection("MSOrderBaseUrl").Value
            };

            PagSeguro pagSeguro = new PagSeguro()
            {
                Account_Id = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Account_Id").Value,
                Client_id = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Client_id").Value,
                Client_secret = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Client_secret").Value,
                Method_Split = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Method_Split").Value,
                Redirect_uri = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Redirect_uri").Value,
                Redirect_url_pam_pos_oauth_pagseguro = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Redirect_url_pam_pos_oauth_pagseguro").Value,
                Token = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Token").Value,
                Url = Configuration.GetSection("MSBillingSettings").GetSection("PagSeguro:Url").Value
            };

            services.AddSingleton((ILogger)new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.File(Path.Combine(logFilePath, logFileName), rollingInterval: RollingInterval.Day)
              .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
              .CreateLogger());

            services.AddScoped<IPaymentRepository, PaymentRepository>(
                provider => new PaymentRepository(connectionString, provider.GetService<ILogger>()));

            services.AddScoped<IDeliveryOptionsRepository, DeliveryOptionsRepository>(
                provider => new DeliveryOptionsRepository(connectionString, provider.GetService<ILogger>()));

            services.AddScoped<ICardRepository, CardRepository>(
                provider => new CardRepository(connectionString, provider.GetService<ILogger>()));

            services.AddScoped<ICardService, CardService>(
                provider => new CardService(
                    provider.GetService<ICardRepository>(),
                    provider.GetService<ILogger>(),
                    privateSecretKey,
                    tokenValidationMinutes,
                    pagSeguro
                )
            );

            services.AddScoped<IPaymentOptionsService, PaymentOptionsService>(
                provider => new PaymentOptionsService(
                    provider.GetService<IPaymentRepository>(),
                    provider.GetService<ILogger>(),
                    privateSecretKey,
                    tokenValidationMinutes,
                    pagSeguro
                )
            );

            services.AddScoped<IDeliveryOptionsService, DeliveryOptionsService>(
                provider => new DeliveryOptionsService(
                    provider.GetService<IDeliveryOptionsRepository>(),
                    provider.GetService<ILogger>(),
                    privateSecretKey,
                    tokenValidationMinutes
                )
            );

            // services.AddTransient<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
            //services.AddTransient<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            // add the Swagger generator and the Swagger UI middlewares   
            app.UseSwaggerUi3();

            app.UseCors(builder =>
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMvc();


        }
    }
}