using Udp.NET.Core.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpErrorServerBaseEventArgs<T> : UdpErrorEventArgs<T> where T : ConnectionUdpServer
    {
    }
}
