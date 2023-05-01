using PHS.Networking.Services;
using Udp.NET.Client.Events.Args;
using Udp.NET.Core.Models;
using Udp.NET.Client.Models;

namespace Udp.NET.Client
{
    public interface IUdpNETClient : 
        ICoreNetworkingClient<
            UdpConnectionClientEventArgs, 
            UdpMessageClientEventArgs, 
            UdpErrorClientEventArgs,
            ConnectionUdpClient>
    {
    }
}