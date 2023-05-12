using PHS.Networking.Server.Services;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server
{
    public interface IUdpNETServerAuth<T> :
         ICoreNetworkingServer<
            UdpConnectionServerAuthEventArgs<T>,
            UdpMessageServerAuthEventArgs<T>,
            UdpErrorServerAuthEventArgs<T>,
            IdentityUdpServer<T>>
    {
        Task SendToUserAsync(string message, T userId, CancellationToken cancellationToken = default);
        Task SendToUserAsync(byte[] message, T userId, CancellationToken cancellationToken = default);

        UdpClient Server { get; }
    }
}