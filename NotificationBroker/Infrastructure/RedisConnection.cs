using StackExchange.Redis;

namespace NotificationPublisher.Infrastructure
{
    public static class RedisConnection
    {
        public static ConnectionMultiplexer Connection { get; } = ConnectionMultiplexer.Connect("localhost");
    }
}