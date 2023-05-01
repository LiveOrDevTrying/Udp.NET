using PHS.Networking.Server.Managers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Managers
{
    public abstract class UdpConnectionManagerAuthBase<Z, A> : ConnectionManager<Z> where Z : IdentityUdpServer<A>
    {
        protected ConcurrentDictionary<A, ConnectionManager<Z>> _users =
            new ConcurrentDictionary<A, ConnectionManager<Z>>();

        public virtual IEnumerable<Z> GetAll(A id)
        {
            if (_users.TryGetValue(id, out var user))
            {
                return user.GetAll();
            }

            return System.Array.Empty<Z>();
        }
        public virtual bool AddIdentity(Z identity)
        {
            if (!_users.TryGetValue(identity.UserId, out var userOriginal))
            {
                userOriginal = new ConnectionManager<Z>();
                if (!_users.TryAdd(identity.UserId, userOriginal))
                {
                    return false;
                }
            }

            var user = new ConnectionManager<Z>(userOriginal.GetAllDictionary());
            user.AddConnection(identity.ConnectionId, identity);
            return _users.TryUpdate(identity.UserId, user, userOriginal);
        }
        public override bool RemoveConnection(string id)
        {
            _connections.TryRemove(id, out var _);

            try
            {
                A userToRemove = default;
                bool removeUser = false;
                foreach (var user in _users)
                {
                    if (user.Value.RemoveConnection(id))
                    {
                        if (user.Value.Count() == 0)
                        {
                            userToRemove = user.Key;
                            removeUser = true;
                            break;
                        }

                        return true;
                    }
                }

                if (removeUser)
                {
                    _users.TryRemove(userToRemove, out var _);
                    return true;
                }
            }
            catch
            { }

            return false;
        }
    }
}
