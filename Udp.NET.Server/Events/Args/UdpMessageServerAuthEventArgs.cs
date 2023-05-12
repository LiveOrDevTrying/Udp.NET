using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpMessageServerAuthEventArgs<T> : UdpMessageServerAuthBaseEventArgs<IdentityUdpServer<T>, T>
    {
    }
}
