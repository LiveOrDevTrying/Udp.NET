using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpMessageServerAuthBaseEventArgs<Z, A> : UdpMessageServerBaseEventArgs<Z> where Z : IdentityUdpServer<A>
    {
    }
}
