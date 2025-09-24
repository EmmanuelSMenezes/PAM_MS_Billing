using Domain.Base;
using Domain.Model;
using Domain.Model.Orders;
using Infrastructure.Repository;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class WorkerService : WorkerBase
    {
        private IPaymentRepository _paymentRepository;
        private Serilog.ILogger _serilog;
        private int _toleranceHour;
        private HttpEndPoints _httpEndPoints = new HttpEndPoints();
        private PagSeguro _pagSeguroAccess = new PagSeguro();
        private static HubConnection connection;
        public WorkerService(ILogger<WorkerService> logger, IOptions<WorkerSettings> config, IOptions<MSBillingSettings> msBillingSettings, IOptions<LogSettings> logSettings, IOptions<HttpEndPoints> httpEndPoint)
            : base(config.Value.DefaultExecutionInterval, logger)
        {
            Serilog.ILogger iLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Combine(logSettings.Value.LogFilePath, logSettings.Value.LogFileNameWorker), rollingInterval: RollingInterval.Day)
                .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
            _serilog = iLogger;
            _toleranceHour = config.Value.ToleranceHour;
            _httpEndPoints = httpEndPoint.Value;
            _pagSeguroAccess = msBillingSettings.Value.PagSeguro;
            ConfigurationRepositorys(msBillingSettings, iLogger);
        }

        private void ConfigurationRepositorys(IOptions<MSBillingSettings> msBillingSettings, Serilog.ILogger iLogger)
        {
            _paymentRepository =
                new PaymentRepository(
                    msBillingSettings.Value.ConnectionString,
                    iLogger);

        }

        protected override async Task RunAsync(object state)
        {
            var orders = _paymentRepository.GetOrdersPix();
            if (orders.Any())
            {
                foreach (var item in orders)
                {
                    var response = await ConsultOrderPagSeguro(item.Id);
                    if (response.Contains("\"status\":\"PAID\""))
                    {
                        await MoveOrderStatus(item, response);
                    }
                    else if (response.Contains("error_messages"))
                    {
                        _serilog.Information($"Erro ao consultar pedido {item.Order_id} no PagSeguro: {response}");
                        _logger.LogInformation($"Erro ao consultar pedido {item.Order_id} no PagSeguro: {response}");

                    }
                }
            }
        }

        private async Task<string> ConsultOrderPagSeguro(string id)
        {
            HttpClient httpClient = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_pagSeguroAccess.Url}orders/{id}"),
                Headers =
                        {
                            { "accept", "application/json" },
                            { "Authorization", $"Bearer {_pagSeguroAccess.Token}" },
                        },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }

        }

        private async Task MoveOrderStatus(OrderPix orderPix,string response)
        {
            connection = new HubConnectionBuilder()
                .WithUrl(@$"{_httpEndPoints.MSOrderBaseUrl}order-status-hub")
                .Build();


            connection.Closed += async (error) =>
            {
                _serilog.Information($"Sem Conexão com signalR de pedidos. Tentando reconectar...");
                _logger.LogInformation($"Sem Conexão com signalR de pedidos. Tentando reconectar...");
                await ReconnectSignalROrder();
            };

            await StartConnectionSignalROrder();

            await connection.InvokeAsync("MoveOrderStatus", orderPix.Order_id, Guid.Parse("36eebcfb-9758-4fdc-ad95-bcdf70703c4a"), orderPix.Created_by);
            _paymentRepository.CreatePaymentHistory(orderPix.Order_id, response, (int)Payment_PagSeguro_Status.PAID);
            await connection.InvokeAsync("MoveOrderStatus", orderPix.Order_id, Guid.Parse("d71cb62a-28dd-44a8-a008-9d7d7d1af810"), orderPix.Created_by);
        }

        private async Task ReconnectSignalROrder()
        {
            const int reconnectDelaySeconds = 5;
            while (true)
            {
                _serilog.Information($"Tentando reconectar em {reconnectDelaySeconds} segundos...");
                _logger.LogInformation($"Tentando reconectar em {reconnectDelaySeconds} segundos...");

                await Task.Delay(TimeSpan.FromSeconds(reconnectDelaySeconds));
                await StartConnectionSignalROrder();
                if (connection.State == HubConnectionState.Connected)
                {
                    _serilog.Information($"Reconexão bem-sucedida com signalR de pedidos.");
                    _logger.LogInformation($"Reconexão bem-sucedida com signalR de pedidos.");
                    break;
                }
            }
        }

        private async Task StartConnectionSignalROrder()
        {
            try
            {
                await connection.StartAsync();
                _serilog.Information($"Conexão com signalR de pedidos: {connection.ConnectionId} iniciada com sucesso.");
                _logger.LogInformation($"Conexão com signalR de pedidos: {connection.ConnectionId} iniciada com sucesso.");

            }
            catch (Exception ex)
            {
                _serilog.Information($"Erro ao iniciar conexão: {ex.Message}");
                _logger.LogInformation($"Erro ao iniciar conexão: {ex.Message}");
                await ReconnectSignalROrder();
            }
        }

    }
}
