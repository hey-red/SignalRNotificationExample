using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Notifications;

using StackExchange.Redis;

namespace NotificationSender
{
    internal class Program
    {
        public static ConnectionMultiplexer Connection { get; } = ConnectionMultiplexer.Connect("localhost");

        private static void Main(string[] args)
        {
            for (int i = 0; i <= 10; i++)
            {
                Connection.GetSubscriber().Publish("PostCreated",
                    JsonConvert.SerializeObject(new PostAddedNotification
                    {
                        PostId = i
                    },
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                    }));

                Console.WriteLine($"Post №{i} sended.");
            }

            Console.ReadKey();
        }
    }
}