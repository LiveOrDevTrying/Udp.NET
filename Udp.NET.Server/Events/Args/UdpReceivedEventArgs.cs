using PHS.Core.Events.Args;
using System.Net.Sockets;
using System.Threading;

namespace Udp.NET.Server.Events.Args
{
    public struct UdpReceivedEventArgs
    {
        public UdpReceiveResult UdpReceiveResult { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
