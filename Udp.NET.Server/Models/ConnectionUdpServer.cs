using PHS.Networking.Server.Models;
using System;
using System.Net;
using Udp.NET.Core.Models;

namespace Udp.NET.Server.Models
{
    public class ConnectionUdpServer : ConnectionUdp, IConnectionServer
    {
        public IPEndPoint IpEndpoint { get; set; }
        public bool HasBeenPinged { get; set; }
        public DateTime NextPing { get; set; }
    }
}
