using PHS.Networking.Events.Args;
using Udp.NET.Core.Models;

namespace Udp.NET.Core.Events.Args
{
    public class UdpMessageEventArgs<T> : MessageEventArgs<T> where T : ConnectionUdp
    {
        public string Message { get; set; }
    }
}
