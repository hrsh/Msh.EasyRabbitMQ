using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace Msh.EasyRabbitMQ.ServiceBus
{
    public class ConnectionManager : IConnectionManager
    {
        private bool _disposed;

        private IConnection _connection;

        readonly object _sync_root = new();

        private readonly ILogger<ConnectionManager> _logger;

        private readonly RabbitMqOptions _options;

        public ConnectionManager(
            ILogger<ConnectionManager> logger,
            IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            _logger = logger;
        }

        private bool IsConnected =>
            _connection is not null && _connection.IsOpen && !_disposed;

        public void Connect()
        {
            if (!IsConnected)
                MakeConnection();
        }

        public void TryConnect()
        {
            if (!IsConnected)
                TryMakeConnection();
        }

        private void MakeConnection()
        {
            var _factory = new ConnectionFactory
            {
                Uri = new(_options.ConnectionString)
            };
            _factory.AutomaticRecoveryEnabled = true;
            _connection = _factory.CreateConnection();

            _connection.ConnectionBlocked += ConnectionBlocked;
            _connection.ConnectionShutdown += ConnectionShutdown;
            _connection.ConnectionUnblocked += ConnectionUnblocked;
        }

        private void TryMakeConnection()
        {
            lock (_sync_root)
            {
                var _factory = new ConnectionFactory
                {
                    Uri = new(_options.ConnectionString)
                };

                Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(
                        _options.RetryCount,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan) =>
                        {
                            _logger.LogWarning(
                                exception,
                                $"RabbitMQ Client could not connect after {timeSpan.TotalSeconds:n1}s ({exception.Message})");
                        })
                    .Execute(() =>
                    {
                        _logger.LogWarning("Try to connect ...");
                        _connection = _factory.CreateConnection();
                    });

                if (IsConnected)
                {
                    _connection.ConnectionBlocked += ConnectionBlocked;
                    _connection.ConnectionShutdown += ConnectionShutdown;
                    _connection.ConnectionUnblocked += ConnectionUnblocked;
                    _connection.CallbackException += ConnectionCallbackException;
                    _logger.LogInformation($"RabbitMQ Client acquired a persistent connection to '{_connection.Endpoint.HostName}' and is subscribed to failure events");
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                }
            }
        }

        private void ConnectionCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void ConnectionUnblocked(object sender, EventArgs e)
        {
            if (_disposed) return;

            _logger.LogInformation("A RabbitMQ connection ubblocked");
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        private void ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        public IModel Channel =>
            IsConnected
            ? _connection.CreateModel()
            : throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _connection?.Close();

            _disposed = true;
        }
    }
}