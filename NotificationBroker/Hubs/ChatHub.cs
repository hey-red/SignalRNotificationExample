using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace NotificationPublisher.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        // Id|Ip
        internal static ConcurrentDictionary<string, string> ConnectedUsers = new();

        public int GetConnectionCounter() => ConnectedUsers.Values
            .Distinct()
            .Count();

        public override async Task OnConnectedAsync()
        {
            var connId = Context.ConnectionId;
            if (!ConnectedUsers.Keys.Any(x => x == connId))
            {
                ConnectedUsers.TryAdd(connId, Context.GetHttpContext().Connection.RemoteIpAddress.ToString());
            }

            await Clients.Groups(ChatHubConstants.GroupName)
                .OnSomeoneConnected();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.TryRemove(Context.ConnectionId, out _);

            await Clients.Groups(ChatHubConstants.GroupName)
                .OnSomeoneDisconnected();

            await base.OnDisconnectedAsync(exception);
        }
    }
}