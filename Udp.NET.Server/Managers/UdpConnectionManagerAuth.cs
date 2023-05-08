using PHS.Networking.Server.Managers;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Managers
{
    public class UdpConnectionManagerAuth<T> : ConnectionManagerAuth<IdentityUdpServer<T>, T>
    {
        public override bool AddUser(IdentityUdpServer<T> identity)
        {
            if (!_connections.TryGetValue(identity.ConnectionId, out var connection))
            {
                _connections.TryUpdate(identity.ConnectionId, identity, connection);
            }

            if (!_users.TryGetValue(identity.UserId, out var userOriginal))
            {
                userOriginal = new ConnectionManager<IdentityUdpServer<T>>();
                if (!_users.TryAdd(identity.UserId, userOriginal))
                {
                    return false;
                }
            }

            var userNew = new ConnectionManager<IdentityUdpServer<T>>(userOriginal.GetAllConnectionsDictionary());
            userNew.AddConnection(identity.ConnectionId, identity);
            return _users.TryUpdate(identity.UserId, userNew, userOriginal);
        }
    }
}
