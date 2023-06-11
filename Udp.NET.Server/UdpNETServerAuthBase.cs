using PHS.Networking.Enums;
using PHS.Networking.Server.Services;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Handlers;
using Udp.NET.Server.Models;
using PHS.Networking.Server.Managers;

namespace Udp.NET.Server
{
    public abstract class UdpNETServerAuthBase<T, U, V, W, X, Y, Z, A> : 
        UdpNETServerBase<T, U, V, W, X, Y, Z>,
        ICoreNetworkingServerAuth<T, U, V, Z, A>
        where T : UdpConnectionServerAuthBaseEventArgs<Z, A>
        where U : UdpMessageServerAuthBaseEventArgs<Z, A>
        where V : UdpErrorServerAuthBaseEventArgs<Z, A>
        where W : ParamsUdpServerAuth
        where X : UdpHandlerServerBase<T, U, V, W, Z>
        where Y : ConnectionManagerAuth<Z, A>
        where Z : IdentityUdpServer<A>
    {
        protected readonly IUserService<A> _userService;

        public UdpNETServerAuthBase(W parameters,
            IUserService<A> userService) : base(parameters)
        { 
            _userService = userService;
        }

        public override async Task<bool> SendToConnectionAsync(string message, Z connection, CancellationToken cancellationToken = default)
        {
            if (connection.Authorized)
            {
                return await base.SendToConnectionAsync(message, connection, cancellationToken);
            }

            return false;
        }
        public override async Task<bool> SendToConnectionAsync(byte[] message, Z connection, CancellationToken cancellationToken = default)
        {
            if (connection.Authorized)
            {
                return await base.SendToConnectionAsync(message, connection, cancellationToken);
            }

            return false;
        }
        public virtual async Task SendToUserAsync(string message, A userId, CancellationToken cancellationToken = default)
        {
            if (IsServerRunning)
            {
                var connections = _connectionManager.GetAllConnectionsForUser(userId);

                foreach (var connection in connections)
                {
                    await SendToConnectionAsync(message, connection, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        public virtual async Task SendToUserAsync(byte[] message, A userId, CancellationToken cancellationToken = default)
        {
            if (IsServerRunning)
            {
                var connections = _connectionManager.GetAllConnectionsForUser(userId);

                foreach (var connection in connections)
                {
                    await SendToConnectionAsync(message, connection, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        protected override void OnConnectionEvent(object sender, T args)
        {
            switch (args.ConnectionEventType)
            {
                case ConnectionEventType.Connected:
                    break;
                case ConnectionEventType.Disconnect:
                    _connectionManager.RemoveConnection(args.Connection.ConnectionId);
                    break;
                default:
                    break;
            }

            FireEvent(this, args);
        }
        protected override void OnMessageEvent(object sender, U args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    break;
                case MessageEventType.Receive:
                    if (!args.Connection.Authorized)
                    {
                        Task.Run(async () =>
                        {
                            if (args.Message.Length > 0 && await _userService.IsValidTokenAsync(args.Bytes, args.CancellationToken).ConfigureAwait(false))
                            {
                                args.Connection.UserId = await _userService.GetIdAsync(args.Bytes, args.CancellationToken).ConfigureAwait(false);
                                args.Connection.Authorized = true;
                                _connectionManager.AddUser(args.Connection);

                                if (!string.IsNullOrWhiteSpace(_parameters.ConnectionSuccessString))
                                {
                                    await SendToConnectionAsync(_parameters.ConnectionSuccessString, args.Connection, args.CancellationToken).ConfigureAwait(false);
                                }

                                FireEvent(this, CreateConnectionEventArgs(new UdpConnectionServerBaseEventArgs<Z>
                                {
                                    CancellationToken = args.CancellationToken,
                                    Connection = args.Connection,
                                    ConnectionEventType = ConnectionEventType.Connected
                                }));
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(_parameters.ConnectionUnauthorizedString))
                                {
                                    await SendToConnectionAsync(_parameters.ConnectionUnauthorizedString, args.Connection, args.CancellationToken).ConfigureAwait(false);
                                
                                    await DisconnectConnectionAsync(args.Connection, args.CancellationToken).ConfigureAwait(false);
                                }
                            }
                        });

                        return;
                    }
                    break;
                default:
                    break;
            }

            base.OnMessageEvent(sender, args);
        }
    }
}
