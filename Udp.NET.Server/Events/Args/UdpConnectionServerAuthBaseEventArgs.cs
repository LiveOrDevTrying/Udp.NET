using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpConnectionServerAuthBaseEventArgs<Z, A> : UdpConnectionServerBaseEventArgs<Z> where Z : IdentityUdpServer<A>
    {
    }
}

