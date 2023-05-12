using PHS.Networking.Models;

namespace Udp.NET.Server.Models
{
    public interface IParamsUdpServer : IParams
    {
        string ConnectionSuccessString { get; }
        byte[] DisconnectBytes { get; }
        bool OnlyEmitBytes { get; }
        byte[] PingBytes { get; }
        int PingIntervalSec { get; }
        byte[] PongBytes { get; }
        int Port { get; }
        byte[] PrefixTerminator { get; }
        bool UseDisconnectBytes { get; }
    }
}