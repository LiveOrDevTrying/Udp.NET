using PHS.Networking.Server.Models;

namespace Udp.NET.Server.Models
{
    public class IdentityUdpServer<T> : ConnectionUdpServer, IIdentity<T>
    {
        public T UserId { get; set; }
        public bool Authorized { get; set; }
    }
}
