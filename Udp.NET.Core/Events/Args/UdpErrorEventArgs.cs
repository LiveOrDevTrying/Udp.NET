using PHS.Networking.Events.Args;
using Udp.NET.Core.Models;

namespace Udp.NET.Core.Events.Args
{
    public class UdpErrorEventArgs<T> : ErrorEventArgs<T> where T : ConnectionUdp
    {
    }
}
