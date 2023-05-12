using PHS.Networking.Models;

namespace Udp.NET.Client.Models
{
    public interface IParamsUdpClient : IParams
    {
        byte[] DisconnectBytes { get; }
        string Host { get; }
        bool OnlyEmitBytes { get; }
        byte[] PingBytes { get; }
        byte[] PongBytes { get; }
        int Port { get; }
        byte[] PrefixTerminator { get; }
        byte[] Token { get; }
        bool UseDisconnectBytes { get; }
        bool UsePingPong { get; }
    }
}