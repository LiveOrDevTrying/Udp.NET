using System.IO;
using System.Net.Sockets;
using Udp.NET.Core.Models;

namespace Udp.NET.Client.Models
{
    public class ConnectionUdpClient : ConnectionUdp
    {
        public Socket Socket { get; set; }

        public override void Dispose()
        {
            base.Dispose();

            try
            {
                Socket.Close();
            }
            catch { }

            try
            {
                Socket.Dispose();
            }
            catch { }
        }
    }
}
