using System;

namespace Udp.NET.Server.Models
{
    public class ParamsUdpServerAuth : ParamsUdpServer
    {
        public string ConnectionUnauthorizedString { get; protected set; }

        public ParamsUdpServerAuth(int port, string connectionSuccessString = null, string connectionUnauthorizedString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, string pingCharacters = "ping", string pongCharacters = "pong", bool sendDisconnectBytes = true, byte[] disconnectBytes = null, string prefixTerminator = "\\") : base(port, connectionSuccessString, onlyEmitBytes, pingIntervalSec, pingCharacters, pongCharacters, sendDisconnectBytes, disconnectBytes, prefixTerminator)
        {
            if (onlyEmitBytes && !string.IsNullOrWhiteSpace(connectionUnauthorizedString))
            {
                throw new ArgumentException("onlyEmitBytes can not be true is a connectionUnauthorizedString is specified");
            }

            ConnectionUnauthorizedString = connectionUnauthorizedString;
        }
    }
}
