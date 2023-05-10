using PHS.Networking.Models;
using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Text;

namespace Udp.NET.Client.Models
{
    public class ParamsUdpClient : Params
    {
        public string Host { get; protected set; }
        public int Port { get; protected set; }
        public byte[] EndOfLineBytes { get; protected set; }
        public bool UsePingPong { get; protected set; }
        public byte[] PingBytes { get; protected set; }
        public byte[] PongBytes { get; protected set; }
        public bool OnlyEmitBytes { get; protected set; }
        public byte[] Token { get; protected set; }
        public bool UseDisconnectBytes { get; protected set; }
        public byte[] DisconnectBytes { get; protected set; }
        public byte[] PrefixTerminator { get; protected set; }

        public ParamsUdpClient(string host, int port, string endOfLineCharacters, string token = "", bool onlyEmitBytes = false, bool usePingPong = true, string pingCharacters = "ping", string pongCharacters = "pong", bool useDisconnectBytes = true, byte[] disconnectBytes = null, string prefixTerminator = "\\")
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Host is not valid");
            }

            if (port <= 0)
            {
                throw new ArgumentException("Port is not valid");
            }

            if (string.IsNullOrEmpty(endOfLineCharacters))
            {
                throw new ArgumentException("End of Line Characters are not valid");
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
            EndOfLineBytes = Encoding.UTF8.GetBytes(endOfLineCharacters);
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
        public ParamsUdpClient(string host, int port, byte[] endOfLineBytes, string token = null, bool onlyEmitBytes = true, bool usePingPong = true, byte[] pingBytes = null, byte[] pongBytes = null, bool useDisconnectBytes = true, byte[] disconnectBytes = null, byte[] prefixTerminator = null)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Host is not valid");
            }

            if (port <= 0)
            {
                throw new ArgumentException("Port is not valid");
            }

            if (endOfLineBytes == null || endOfLineBytes.Length <= 0 || Statics.ByteArrayEquals(endOfLineBytes, Array.Empty<byte>()))
            {
                throw new ArgumentException("End of Line Characters are not valid");
            }

            if (token != null && string.IsNullOrWhiteSpace(token))
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
            EndOfLineBytes = endOfLineBytes;
            UsePingPong = usePingPong;
            PingBytes = pingBytes;
            PongBytes = pongBytes;
            OnlyEmitBytes = onlyEmitBytes;
            UseDisconnectBytes = useDisconnectBytes;
            DisconnectBytes = disconnectBytes;
            PrefixTerminator = prefixTerminator;

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
