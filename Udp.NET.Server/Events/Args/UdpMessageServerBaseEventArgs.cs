using Udp.NET.Core.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Events.Args
{
    public class UdpMessageServerBaseEventArgs<T> : UdpMessageEventArgs<T> where T : ConnectionUdpServer
    {
    }
}
