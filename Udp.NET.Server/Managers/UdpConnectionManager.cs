using PHS.Networking.Server.Managers;
using System.Collections.Concurrent;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Managers
{
    public class UdpConnectionManager : ConnectionManager<ConnectionUdpServer>
    {
        public UdpConnectionManager() { }
        public UdpConnectionManager(ConcurrentDictionary<string, ConnectionUdpServer> connections) : base(connections) { }
    }
}
