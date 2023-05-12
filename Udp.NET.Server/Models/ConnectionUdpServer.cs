using System;
using System.Net;
using Udp.NET.Core.Models;

namespace Udp.NET.Server.Models
{
    public class ConnectionUdpServer : ConnectionUdp
    {
        public IPEndPoint IpEndpoint { get; set; }
        public bool HasBeenPinged { get; set; }
        public DateTime NextPing { get; set; }
        public bool Disposed { get; set; }
    }
}
