using PHS.Networking.Services;
using Udp.NET.Client.Events.Args;
using Udp.NET.Client.Handlers;
using Udp.NET.Client.Models;

namespace Udp.NET.Client
{
    public class UdpNETClient :
        CoreNetworkingClient<
            UdpConnectionClientEventArgs,
            UdpMessageClientEventArgs,
            UdpErrorClientEventArgs,
            ParamsUdpClient,
            UdpClientHandler,
            ConnectionUdpClient>,
        IUdpNETClient
    {
        public UdpNETClient(ParamsUdpClient parameters) : base(parameters)
        {
        }

        protected override UdpClientHandler CreateHandler()
        {
            return new UdpClientHandler(_parameters);
        }
    }
}
