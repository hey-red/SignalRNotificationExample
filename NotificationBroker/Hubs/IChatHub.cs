using System.Threading.Tasks;

using Notifications;

namespace NotificationPublisher.Hubs
{
    public interface IChatHub
    {
        Task ReceivePost(PostAddedNotification notification);

        Task OnSomeoneConnected();

        Task OnSomeoneDisconnected();
    }
}