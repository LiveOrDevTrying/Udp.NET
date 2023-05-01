using Udp.NET.Core.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpConnectionServerBaseEventArgs<T> : UdpConnectionEventArgs<T> where T : ConnectionUdpServer
    {
    }
}

