﻿using PHS.Networking.Models;
using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Text;

namespace Udp.NET.Server.Models
{
    public class ParamsUdpServer : ParamsPort
    {
        public byte[] EndOfLineBytes { get; protected set; }
        public byte[] PingBytes { get; protected set; }
        public byte[] PongBytes { get; protected set; }
        public string ConnectionSuccessString { get; protected set; }
        public int PingIntervalSec { get; protected set; }
        public bool OnlyEmitBytes { get; protected set; }
        public bool UseDisconnectBytes { get; protected set; }
        public byte[] DisconnectBytes { get; protected set; }
        public byte[] PrefixTerminator { get; protected set; }

        public ParamsUdpServer(int port, string endOfLineCharacters, string connectionSuccessString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, string pingCharacters = "ping", string pongCharacters = "pong", bool useDisconnectBytes = true, byte[] disconnectBytes = null, string prefixTerminator = "\\") : base(port)
        {
            if (string.IsNullOrEmpty(endOfLineCharacters))
            {
                throw new ArgumentException("End of Line Characters are not valid");
            }

            if (string.IsNullOrEmpty(pingCharacters))
            {
                throw new ArgumentException("Ping Characters are not valid");
            }

            if (string.IsNullOrEmpty(pongCharacters))
            {
                throw new ArgumentException("Pong Characters are not valid");
            }

            if (onlyEmitBytes && !string.IsNullOrWhiteSpace(connectionSuccessString))
            {
                throw new ArgumentException("onlyEmitBytes can not be true is a connectionSuccesString is specified");
            }

            if (string.IsNullOrWhiteSpace(prefixTerminator))
            {
                throw new ArgumentException("Prefix Terminator is not valid");
            }

            EndOfLineBytes = Encoding.UTF8.GetBytes(endOfLineCharacters);
            PingBytes = Encoding.UTF8.GetBytes(pingCharacters);
            PongBytes = Encoding.UTF8.GetBytes(pongCharacters);
            ConnectionSuccessString = connectionSuccessString;
            PingIntervalSec = pingIntervalSec;
            OnlyEmitBytes = onlyEmitBytes;
            UseDisconnectBytes = useDisconnectBytes;
            DisconnectBytes = disconnectBytes;
            PrefixTerminator = Encoding.UTF8.GetBytes(prefixTerminator);
            
            if (UseDisconnectBytes && (DisconnectBytes == null || Statics.ByteArrayEquals(DisconnectBytes, Array.Empty<byte>())))
            {
                DisconnectBytes = new byte[] { 3 };
            }
        }

        public ParamsUdpServer(int port, byte[] endOfLineBytes, string connectionSuccessString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, byte[] pingBytes = null, byte[] pongBytes = null, bool useDisconnectBytes = true, byte[] disconnectBytes = null, byte[] prefixTerminator = null) : base(port)
        {
            if (endOfLineBytes.Length <= 0 || endOfLineBytes.All(x => x == 0))
            {
                throw new ArgumentException("End of Line Characters are not valid");
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
            EndOfLineBytes = endOfLineBytes;
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
