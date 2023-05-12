using PHS.Networking.Enums;
using PHS.Networking.Handlers;
using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Client.Models;
using Udp.NET.Core.Events.Args;

namespace Udp.NET.Client.Handlers
{
    public abstract class UdpClientHandlerBase<T, U, V, W, Y> : 
        HandlerClientBase<T, U, V, W, Y>
        where T : UdpConnectionEventArgs<Y>
        where U : UdpMessageEventArgs<Y>
        where V : UdpErrorEventArgs<Y>
        where W : ParamsUdpClient
        where Y : ConnectionUdpClient
    {
        public UdpClientHandlerBase(W parameters) : base(parameters)
        {
        }
        
        public override async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (_connection != null)
                    {
                        await DisconnectAsync(cancellationToken).ConfigureAwait(false);
                    }

                    await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
                    
                    if (_connection != null && _connection.Socket.Connected && !cancellationToken.IsCancellationRequested)
                    {
                        FireEvent(this, CreateConnectionEventArgs(new UdpConnectionEventArgs<Y>
                        {
                            Connection = _connection,
                            ConnectionEventType = ConnectionEventType.Connected,
                            CancellationToken = cancellationToken
                        }));

                        _ = Task.Run(async () => { await ReceiveAsync(cancellationToken).ConfigureAwait(false); }, cancellationToken).ConfigureAwait(false);

                        if (_parameters.Token != null && !cancellationToken.IsCancellationRequested)
                        {
                            await SendAsync(_parameters.Token, cancellationToken).ConfigureAwait(false);
                        }

                        return true;
                    };
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorEventArgs<Y>
                {
                    Exception = ex,
                    Connection = _connection,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));

                await DisconnectAsync(cancellationToken).ConfigureAwait(false);
            }

            return false;
        }
        public override async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection != null)
                {
                    if (_connection.Socket != null)
                    {
                        if (_parameters.UseDisconnectBytes)
                        {
                            await SendAsync(_parameters.DisconnectBytes, cancellationToken).ConfigureAwait(false);
                        }

                        _connection?.Dispose();

                        FireEvent(this, CreateConnectionEventArgs(new UdpConnectionEventArgs<Y>
                        {
                            ConnectionEventType = ConnectionEventType.Disconnect,
                            Connection = _connection,
                            CancellationToken = cancellationToken
                        }));
                    }

                    _connection = null;

                    return true;
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorEventArgs<Y>
                {
                    Connection = _connection,
                    Exception = ex,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));
            }

            return false;
        }

        public override async Task<bool> SendAsync(string message, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection != null &&
                    _connection.Socket != null &&
                    _connection.Socket.Connected &&
                    !cancellationToken.IsCancellationRequested)
                {
                    var bytes = Encoding.UTF8.GetBytes(_connection.ConnectionId).Concat(_parameters.PrefixTerminator).Concat(Encoding.UTF8.GetBytes(message)).ToArray();
                    await _connection.Socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None, cancellationToken).ConfigureAwait(false);

                    FireEvent(this, CreateMessageEventArgs(new UdpMessageEventArgs<Y>
                    {
                        MessageEventType = MessageEventType.Sent,
                        Connection = _connection,
                        Message = message,
                        Bytes = bytes,
                        CancellationToken = cancellationToken
                    }));

                    return true;
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorEventArgs<Y>
                {
                    Connection = _connection,
                    Exception = ex,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));

                await DisconnectAsync(cancellationToken).ConfigureAwait(false);
            }

            return false;
        }
        public override async Task<bool> SendAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection != null &&
                    _connection.Socket != null &&
                    _connection.Socket.Connected &&
                    !cancellationToken.IsCancellationRequested)
                {
                    var bytes = Encoding.UTF8.GetBytes(_connection.ConnectionId).Concat(_parameters.PrefixTerminator).Concat(message).ToArray();
                    await _connection.Socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None, cancellationToken).ConfigureAwait(false);

                    FireEvent(this, CreateMessageEventArgs(new UdpMessageEventArgs<Y>
                    { 
                        MessageEventType = MessageEventType.Sent,
                        Connection = _connection,
                        Message = null,
                        Bytes = bytes,
                        CancellationToken = cancellationToken
                    }));

                    return true;
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorEventArgs<Y>
                {
                    Connection = _connection,
                    Exception = ex,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));

                await DisconnectAsync(cancellationToken).ConfigureAwait(false);
            }

            return false;
        }

        protected virtual async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _connection != null && _connection.Socket != null && _connection.Socket.Connected)
                {
                    if (_connection.Socket.Available <= 0)
                    {
                        await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    var buffer = new byte[_connection.Socket.Available];
                    var result = await _connection.Socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);

                    var prefix = Statics.ByteArraySeparate(buffer, _parameters.PrefixTerminator);

                    if (Encoding.UTF8.GetString(prefix[0]) == _connection.ConnectionId)
                    {
                        buffer = buffer.Skip(prefix[0].Length + _parameters.PrefixTerminator.Length).ToArray();

                        if (_parameters.UseDisconnectBytes && Statics.ByteArrayEquals(buffer, _parameters.DisconnectBytes))
                        {
                            _connection?.Dispose();

                            FireEvent(this, CreateConnectionEventArgs(new UdpConnectionEventArgs<Y>
                            {
                                ConnectionEventType = ConnectionEventType.Disconnect,
                                Connection = _connection,
                                CancellationToken = cancellationToken
                            }));

                            _connection = null;
                            return;
                        }
                        else if (_parameters.UsePingPong && Statics.ByteArrayEquals(buffer, _parameters.PingBytes))
                        {
                            await SendAsync(_parameters.PongBytes, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            FireEvent(this, CreateMessageEventArgs(new UdpMessageEventArgs<Y>
                            {
                                MessageEventType = MessageEventType.Receive,
                                Connection = _connection,
                                Message = !_parameters.OnlyEmitBytes ? Encoding.UTF8.GetString(buffer) : null,
                                Bytes = buffer,
                                CancellationToken = cancellationToken
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FireEvent(this, CreateErrorEventArgs(new UdpErrorEventArgs<Y>
                {
                    Connection = _connection,
                    Exception = ex,
                    Message = ex.Message,
                    CancellationToken = cancellationToken
                }));
            }

            await DisconnectAsync(cancellationToken).ConfigureAwait(false);
        }

        protected virtual async Task CreateConnectionAsync(CancellationToken cancellationToken)
        {
            // Establish the remote endpoint for the socket.  
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 60000
            };

            await socket.ConnectAsync(_parameters.Host, _parameters.Port, cancellationToken).ConfigureAwait(false);
            
            _connection = CreateConnection(new ConnectionUdpClient
            {
                Socket = socket,
                ConnectionId = Guid.NewGuid().ToString()
            });
        }
        protected abstract Y CreateConnection(ConnectionUdpClient connection);
        protected abstract T CreateConnectionEventArgs(UdpConnectionEventArgs<Y> args);
        protected abstract U CreateMessageEventArgs(UdpMessageEventArgs<Y> args);
        protected abstract V CreateErrorEventArgs(UdpErrorEventArgs<Y> args);
    }
}
