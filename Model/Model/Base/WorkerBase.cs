using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Base
{
    public abstract class WorkerBase : IHostedService, IDisposable
    {
        private readonly object _lock = new object();
        private readonly string _workerName;
        private bool _disposed;
        private Timer _timer;
        private bool _isRunning;
        private readonly int _secondsToStart;
        private readonly int _secondsToRunPeriodically;
        protected readonly ILogger<WorkerBase> _logger;

        protected WorkerBase(int secondsToRunPeriodically, ILogger<WorkerBase> logger)
            : this(0, secondsToRunPeriodically, logger) { }

        protected WorkerBase(int secondsToStart, int secondsToRunPeriodically, ILogger<WorkerBase> logger)
        {
            _isRunning = false;
            _secondsToStart = secondsToStart;
            _secondsToRunPeriodically = secondsToRunPeriodically;
            _logger = logger;
            _workerName = GetType().Name;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The Worker {WorkerName} has been started. Itt will be executed every {SecondsToRunPeriodically} seconds.", _workerName, _secondsToRunPeriodically);
            _timer = new Timer(RunBase, null, TimeSpan.FromSeconds(_secondsToStart), TimeSpan.FromSeconds(_secondsToRunPeriodically));
            return Task.CompletedTask;
        }

        private void RunBase(object state)
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    _logger.LogInformation("The worker {WorkerName} is already running", _workerName);
                    return;
                }
                _isRunning = true;
            }
            _ = RunBaseAsync(state);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task RunBaseAsync(object state)
        {
            try
            {
                _logger.LogInformation("The worker {WorkerName} will be executed.", _workerName);
                await RunAsync(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The error {ErrorMessage} occurred when runing {WorkerName}", ex.Message, _workerName);
            }
            finally
            {
                _isRunning = false;
                _logger.LogInformation("The worker {WorkerName} has been finished.", _workerName);
            }
        }

        protected abstract Task RunAsync(object state);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing) _timer?.Dispose();

            _disposed = true;
        }
    }
}