using System;

namespace Udp.NET.Server.Models
{
    public class ParamsUdpServerAuthBytes : ParamsUdpServerBytes
    {
        public string ConnectionUnauthorizedString { get; protected set; }

        public ParamsUdpServerAuthBytes(int port, string connectionSuccessString = null, string connectionUnauthorizedString = null, bool onlyEmitBytes = false, int pingIntervalSec = 120, byte[] pingBytes = null, byte[] pongBytes = null, bool sendDisconnectBytes = true, byte[] disconnectBytes = null, byte[] prefixTerminator = null) : base(port, connectionSuccessString, onlyEmitBytes, pingIntervalSec, pingBytes, pongBytes, sendDisconnectBytes, disconnectBytes, prefixTerminator)
        {
            if (onlyEmitBytes && !string.IsNullOrWhiteSpace(connectionUnauthorizedString))
            {
                throw new ArgumentException("onlyEmitBytes can not be true is a connectionUnauthorizedString is specified");
            }

            ConnectionUnauthorizedString = connectionUnauthorizedString;
        }
    }
}
