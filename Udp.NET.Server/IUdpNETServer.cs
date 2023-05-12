using PHS.Networking.Server.Services;
using System.Net.Sockets;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server
{
    public interface IUdpNETServer :
        ICoreNetworkingServer<
            UdpConnectionServerEventArgs, 
            UdpMessageServerEventArgs, 
            UdpErrorServerEventArgs,
            ConnectionUdpServer>
    {
        UdpClient Server { get; }
    }
}