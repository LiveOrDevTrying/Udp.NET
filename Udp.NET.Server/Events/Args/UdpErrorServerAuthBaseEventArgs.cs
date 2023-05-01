using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpErrorServerAuthBaseEventArgs<Z, A> : UdpErrorServerBaseEventArgs<Z> where Z : IdentityUdpServer<A>
    {
    }
}
