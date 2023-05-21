using PHS.Networking.Models;

namespace Udp.NET.Core.Models
{
    public class ConnectionUdp : IConnection
    {
        public string ConnectionId { get; set; }

        public bool Disposed { get; set; }

        public virtual void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
            }
        }
    }
}
