using System;

namespace Udp.NET.Server.Models
{
    public class ParamsUdpServerAuth : ParamsUdpServer
    {
        public string ConnectionUnauthorizedString { get; protected set; }

        public ParamsUdpServerAuth(int port, string endOfLineCharacters, string connectionSuccessString = null, string connectionUnauthorizedString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, string pingCharacters = "ping", string pongCharacters = "pong", bool sendDisconnectBytes = true, byte[] disconnectBytes = null) : base(port, endOfLineCharacters, connectionSuccessString, onlyEmitBytes, pingIntervalSec, pingCharacters, pongCharacters, sendDisconnectBytes, disconnectBytes)
        {
            if (onlyEmitBytes && !string.IsNullOrWhiteSpace(connectionUnauthorizedString))
            {
                throw new ArgumentException("onlyEmitBytes can not be true is a connectionUnauthorizedString is specified");
            }

            ConnectionUnauthorizedString = connectionUnauthorizedString;
        }

        public ParamsUdpServerAuth(int port, byte[] endOfLineBytes, string connectionSuccessString = null, string connectionUnauthorizedString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, byte[] pingBytes = null, byte[] pongBytes = null, bool sendDisconnectBytes = true, byte[] disconnectBytes = null) : base(port, endOfLineBytes, connectionSuccessString, onlyEmitBytes, pingIntervalSec, pingBytes, pongBytes, sendDisconnectBytes, disconnectBytes)
        {
            if (onlyEmitBytes && !string.IsNullOrWhiteSpace(connectionUnauthorizedString))
            {
                throw new ArgumentException("onlyEmitBytes can not be true is a connectionUnauthorizedString is specified");
            }

            ConnectionUnauthorizedString = connectionUnauthorizedString;
        }
    }
}
