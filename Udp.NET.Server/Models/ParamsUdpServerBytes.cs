using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Text;

namespace Udp.NET.Server.Models
{
    public class ParamsUdpServerBytes : IParamsUdpServer
    {
        public int Port { get; protected set; }
        public byte[] PingBytes { get; protected set; }
        public byte[] PongBytes { get; protected set; }
        public string ConnectionSuccessString { get; protected set; }
        public int PingIntervalSec { get; protected set; }
        public bool OnlyEmitBytes { get; protected set; }
        public bool UseDisconnectBytes { get; protected set; }
        public byte[] DisconnectBytes { get; protected set; }
        public byte[] PrefixTerminator { get; protected set; }

        public ParamsUdpServerBytes(int port, string connectionSuccessString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, byte[] pingBytes = null, byte[] pongBytes = null, bool useDisconnectBytes = true, byte[] disconnectBytes = null, byte[] prefixTerminator = null) : base()
        {
            if (port <= 0)
            {
                throw new ArgumentException("Port is not valid");
            }

            if (pingBytes == null || pingBytes.Length <= 0 || pingBytes.All(x => x == 0))
            {
                pingBytes = Encoding.UTF8.GetBytes("ping");
            }

            if (pongBytes == null || pongBytes.Length <= 0 || pingBytes.All(x => x == 0))
            {
                pongBytes = Encoding.UTF8.GetBytes("pong");
            }

            if (onlyEmitBytes && !string.IsNullOrWhiteSpace(connectionSuccessString))
            {
                throw new ArgumentException("onlyEmitBytes can not be true is a connectionSuccesString is specified");
            }

            if (prefixTerminator == null || prefixTerminator.Where(x => x != 0).ToArray().Length <= 0)
            {
                prefixTerminator = Encoding.UTF8.GetBytes("\\");
            }

            Port = port;
            ConnectionSuccessString = connectionSuccessString;
            PingBytes = pingBytes;
            PongBytes = pongBytes;
            PingIntervalSec = pingIntervalSec;
            OnlyEmitBytes = onlyEmitBytes;
            UseDisconnectBytes = useDisconnectBytes;
            DisconnectBytes = disconnectBytes;
            PrefixTerminator = prefixTerminator;

            if (UseDisconnectBytes && (DisconnectBytes == null || Statics.ByteArrayEquals(DisconnectBytes, Array.Empty<byte>())))
            {
                DisconnectBytes = new byte[] { 3 };
            }
        }
    }
}
