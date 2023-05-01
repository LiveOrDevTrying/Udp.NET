using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpErrorServerAuthEventArgs<T> : UdpErrorServerAuthBaseEventArgs<IdentityUdpServer<T>, T>
    {
    }
}
