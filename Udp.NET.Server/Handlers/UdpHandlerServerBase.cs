using PHS.Networking.Enums;
using PHS.Networking.Events.Args;
using PHS.Networking.Server.Enums;
using PHS.Networking.Server.Events.Args;
using PHS.Networking.Server.Handlers;
using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Handlers
{
    public delegate void ReceivedEvent(object sender,  UdpReceivedEventArgs e);

    public abstract class UdpHandlerServerBase<T, U, V, W, Z> :
        HandlerServerBase<T, U, V, W, Z>
        where T : UdpConnectionServerBaseEventArgs<Z>
        where U : UdpMessageServerBaseEventArgs<Z>
        where V : UdpErrorServerBaseEventArgs<Z>
        where W : ParamsUdpServer
        where Z : ConnectionUdpServer
    {
        protected UdpClient _server;

        private event ReceivedEvent _receivedEvent;

        public UdpHandlerServerBase(W parameters) : base(parameters)
        {
        }

        public override void Start(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_server != null)
                {
                    Stop(cancellationToken);
                }

                _isRunning = true;

                _server = new UdpClient(_parameters.Port);

                FireEvent(this, new ServerEventArgs
                {
                    ServerEventType = ServerEventType.Start,
                    CancellationToken = cancellationToken
                });

                _ = Task.Run(async () => { await ListenForConnectionsAsync(cancellationToken).ConfigureAwait(false); }, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new ErrorEventArgs<Z>
                {
                    Exception = ex,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));
            }
        }
        public override void Stop(CancellationToken cancellationToken = default)
        {
            _isRunning = false;

            try
            {
                if (_server != null)
                {
                    _server.Close();
                    _server = null;
                }

                FireEvent(this, new ServerEventArgs
                {
                    ServerEventType = ServerEventType.Stop,
                    CancellationToken = cancellationToken
                });
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new ErrorEventArgs<Z>
                {
                    Exception = ex,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));
            }
        }

        protected virtual async Task ListenForConnectionsAsync(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var data = await _server.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    _receivedEvent?.Invoke(this, new UdpReceivedEventArgs
                    {
                        UdpReceiveResult = data,
                        CancellationToken = cancellationToken
                    });
                }
                catch (Exception ex)
                {
                    FireEvent(this, CreateErrorEventArgs(new ErrorEventArgs<Z>
                    {
                        Exception = ex,
                        Message = ex.Message,
                        CancellationToken = cancellationToken
                    }));
                }
            }
        }

        public override async Task<bool> SendAsync(string message, Z connection, CancellationToken cancellationToken)
        {
            try
            {
                if (_isRunning && !cancellationToken.IsCancellationRequested && !string.IsNullOrWhiteSpace(message))
                {
                    var bytes = Encoding.UTF8.GetBytes(connection.ConnectionId).Concat(_parameters.PrefixTerminator).Concat(Encoding.UTF8.GetBytes(message)).ToArray();
                    await _server.SendAsync(bytes, bytes.Length, connection.IpEndpoint).ConfigureAwait(false);

                    FireEvent(this, CreateMessageEventArgs(new UdpMessageServerBaseEventArgs<Z>
                    {
                        MessageEventType = MessageEventType.Sent,
                        Connection = connection,
                        Message = message,
                        Bytes = bytes,
                        CancellationToken = cancellationToken
                    }));

                    return true;
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorServerBaseEventArgs<Z>
                {
                    Exception = ex,
                    Message = ex.Message,
                    Connection = connection,
                    CancellationToken = cancellationToken
                }));
            }

            await DisconnectConnectionAsync(connection, cancellationToken).ConfigureAwait(false);

            return false;
        }
        public override async Task<bool> SendAsync(byte[] message, Z connection, CancellationToken cancellationToken)
        {
            try
            {
                if (_isRunning && !cancellationToken.IsCancellationRequested && message.Where(x => x != 0).Any())
                {
                    var bytes = Encoding.UTF8.GetBytes(connection.ConnectionId).Concat(_parameters.PrefixTerminator).Concat(message).ToArray();
                    await _server.SendAsync(bytes, bytes.Length, connection.IpEndpoint).ConfigureAwait(false);

                    FireEvent(this, CreateMessageEventArgs(new UdpMessageServerBaseEventArgs<Z>
                    {
                        MessageEventType = MessageEventType.Sent,
                        Connection = connection,
                        Message = null,
                        Bytes = bytes,
                        CancellationToken = cancellationToken
                    }));

                    return true;
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorServerBaseEventArgs<Z>
                {
                    Exception = ex,
                    Message = ex.Message,
                    Connection = connection,
                    CancellationToken = cancellationToken
                }));
            }

            await DisconnectConnectionAsync(connection, cancellationToken).ConfigureAwait(false);

            return false;
        }
        public override async Task<bool> DisconnectConnectionAsync(Z connection, CancellationToken cancellationToken = default, string disconnectMessage = "")
        {
            try
            {
                if (!connection.Disposed)
                {
                    connection.Disposed = true;

                    if (!string.IsNullOrWhiteSpace(disconnectMessage))
                    {
                        await SendAsync(disconnectMessage, connection, cancellationToken).ConfigureAwait(false);
                    }

                    if (_parameters.UseDisconnectBytes)
                    {
                        await SendAsync(_parameters.DisconnectBytes, connection, cancellationToken).ConfigureAwait(false);
                    }

                    connection?.Dispose();

                    FireEvent(this, CreateConnectionEventArgs(new UdpConnectionServerBaseEventArgs<Z>
                    {
                        ConnectionEventType = ConnectionEventType.Disconnect,
                        Connection = connection,
                        CancellationToken = cancellationToken
                    }));

                    return true;
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorServerBaseEventArgs<Z>
                {
                    Exception = ex,
                    Message = ex.Message,
                    Connection = connection,
                    CancellationToken = cancellationToken
                }));
            }

            return false;
        }

        public virtual void Receive(byte[] message, Z connection, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_parameters.UseDisconnectBytes && Statics.ByteArrayEquals(message, _parameters.DisconnectBytes))
                {
                    connection?.Dispose();

                    FireEvent(this, CreateConnectionEventArgs(new UdpConnectionServerBaseEventArgs<Z>
                    {
                        ConnectionEventType = ConnectionEventType.Disconnect,
                        Connection = connection,
                        CancellationToken = cancellationToken
                    }));

                    return;
                }
                else if (Statics.ByteArrayEquals(message, _parameters.PongBytes))
                {
                    connection.HasBeenPinged = false;
                }
                else
                {
                    FireEvent(this, CreateMessageEventArgs(new UdpMessageServerBaseEventArgs<Z>
                    {
                        Connection = connection,
                        Message = !_parameters.OnlyEmitBytes ? Encoding.UTF8.GetString(message) : null,
                        MessageEventType = MessageEventType.Receive,
                        Bytes = message,
                        CancellationToken = cancellationToken
                    }));
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorServerBaseEventArgs<Z>
                {
                    Exception = ex,
                    Message = ex.Message,
                    Connection = connection,
                    CancellationToken = cancellationToken
                }));
            }
        }

        protected abstract T CreateConnectionEventArgs(ConnectionEventArgs<Z> args);
        protected abstract V CreateErrorEventArgs(ErrorEventArgs<Z> args);
        protected abstract U CreateMessageEventArgs(UdpMessageServerBaseEventArgs<Z> args);

        protected virtual void FireEvent(object sender, UdpReceivedEventArgs args)
        {
            _receivedEvent?.Invoke(sender, args);
        }

        public event ReceivedEvent ReceivedEvent
        {
            add
            {
                _receivedEvent += value;
            }
            remove
            {
                _receivedEvent -= value;
            }
        }
        public UdpClient Server
        {
            get
            {
                return _server;
            }
        }
    }
}