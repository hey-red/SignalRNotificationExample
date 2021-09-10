using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NotificationPublisher.Hubs;
using NotificationPublisher.Infrastructure;

using Notifications;

namespace NotificationPublisher.Services
{
    public class NotificationBroker : BackgroundService
    {
        private readonly IHubContext<ChatHub, IChatHub> _chatHubContext;

        private readonly ILogger _logger;

        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public NotificationBroker(
            IHubContext<ChatHub, IChatHub> chatdHubContext,
            ILoggerFactory loggerFactory)
        {
            _chatHubContext = chatdHubContext;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        private readonly string ServiceName = nameof(NotificationBroker);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{ServiceName} is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation($"{ServiceName} background task is stopping."));

            // Subscribe to channels
            RedisConnection.Connection.GetSubscriber().Subscribe("PostCreated", async (channel, message) =>
            {
                _logger.LogInformation($"PostAdded -> {message}");

                var notification = JsonSerializer
                    .Deserialize<PostAddedNotification>(message, jsonOptions);

                await _chatHubContext.Clients
                    .Group(ChatHubConstants.GroupName)
                    .ReceivePost(notification);
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);

            _logger.LogDebug($"{ServiceName} is stopping.");
        }
    }
}