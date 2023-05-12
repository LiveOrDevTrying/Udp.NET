using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Text;

namespace Udp.NET.Client.Models
{
    public class ParamsUdpClientBytes : IParamsUdpClient
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

        public ParamsUdpClientBytes(string host, int port, byte[] token = null, bool onlyEmitBytes = true, bool usePingPong = true, byte[] pingBytes = null, byte[] pongBytes = null, bool useDisconnectBytes = true, byte[] disconnectBytes = null, byte[] prefixTerminator = null)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Host is not valid");
            }

            if (port <= 0)
            {
                throw new ArgumentException("Port is not valid");
            }

            if (token != null && token.Where(x => x != 0).ToArray().Length <= 0)
            {
                throw new ArgumentException("Token is not valid");
            }

            if (usePingPong && (pingBytes == null || pingBytes.Length <= 0 || Statics.ByteArrayEquals(pingBytes, Array.Empty<byte>())))
            {
                pingBytes = Encoding.UTF8.GetBytes("ping");
            }

            if (usePingPong && (pongBytes == null || pongBytes.Length <= 0 || Statics.ByteArrayEquals(pongBytes, Array.Empty<byte>())))
            {
                pongBytes = Encoding.UTF8.GetBytes("pong");
            }

            if (prefixTerminator == null || prefixTerminator.Where(x => x != 0).ToArray().Length <= 0)
            {
                prefixTerminator = Encoding.UTF8.GetBytes("\\");
            }

            Host = host;
            Port = port;
            UsePingPong = usePingPong;
            PingBytes = pingBytes;
            PongBytes = pongBytes;
            OnlyEmitBytes = onlyEmitBytes;
            UseDisconnectBytes = useDisconnectBytes;
            DisconnectBytes = disconnectBytes;
            PrefixTerminator = prefixTerminator;
            Token = token;

            if (UseDisconnectBytes && (DisconnectBytes == null || Statics.ByteArrayEquals(DisconnectBytes, Array.Empty<byte>())))
            {
                DisconnectBytes = new byte[] { 3 };
            }
        }
    }
}
