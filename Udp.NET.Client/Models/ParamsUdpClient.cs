using PHS.Networking.Utilities;
using System;
using System.Text;

namespace Udp.NET.Client.Models
{
    public class ParamsUdpClient : IParamsUdpClient
    {
        public string Host { get; protected set; }
        public int Port { get; protected set; }
        public bool UsePingPong { get; protected set; }
        public byte[] PingBytes { get; protected set; }
        public byte[] PongBytes { get; protected set; }
        public bool OnlyEmitBytes { get; protected set; }
        public byte[] Token { get; protected set; }
        public bool UseDisconnectBytes { get; protected set; }
        public byte[] DisconnectBytes { get; protected set; }
        public byte[] PrefixTerminator { get; protected set; }

        public ParamsUdpClient(string host, int port, string token = "", bool onlyEmitBytes = false, bool usePingPong = true, string pingCharacters = "ping", string pongCharacters = "pong", bool useDisconnectBytes = true, byte[] disconnectBytes = null, string prefixTerminator = "\\")
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Host is not valid");
            }

            if (port <= 0)
            {
                throw new ArgumentException("Port is not valid");
            }

            if (usePingPong && string.IsNullOrEmpty(pingCharacters))
            {
                throw new ArgumentException("Ping Characters are not valid");
            }

            if (usePingPong && string.IsNullOrEmpty(pongCharacters))
            {
                throw new ArgumentException("Pong Characters are not valid");
            }

            if (string.IsNullOrWhiteSpace(prefixTerminator))
            {
                throw new ArgumentException("Prefix Terminator is not valid");
            }

            Host = host;
            Port = port;
            UsePingPong = usePingPong;
            PingBytes = Encoding.UTF8.GetBytes(pingCharacters);
            PongBytes = Encoding.UTF8.GetBytes(pongCharacters);
            OnlyEmitBytes = onlyEmitBytes;
            UseDisconnectBytes = useDisconnectBytes;
            DisconnectBytes = disconnectBytes;
            PrefixTerminator = Encoding.UTF8.GetBytes(prefixTerminator);

            if (!string.IsNullOrWhiteSpace(token))
            {
                Token = Encoding.UTF8.GetBytes(token);
            }

            if (UseDisconnectBytes && (DisconnectBytes == null || Statics.ByteArrayEquals(DisconnectBytes, Array.Empty<byte>())))
            {
                DisconnectBytes = new byte[] { 3 };
            }
        }
    }
}
