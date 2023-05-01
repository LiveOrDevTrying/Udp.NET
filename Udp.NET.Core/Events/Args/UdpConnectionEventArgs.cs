using PHS.Networking.Events.Args;
using Udp.NET.Core.Models;

namespace Udp.NET.Core.Events.Args
{
    public class UdpConnectionEventArgs<T> : ConnectionEventArgs<T> where T : ConnectionUdp
    {
    }
}
