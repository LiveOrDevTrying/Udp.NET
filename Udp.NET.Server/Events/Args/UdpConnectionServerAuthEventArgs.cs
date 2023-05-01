using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpConnectionServerAuthEventArgs<T> : UdpConnectionServerAuthBaseEventArgs<IdentityUdpServer<T>, T>
    {
    }
}

