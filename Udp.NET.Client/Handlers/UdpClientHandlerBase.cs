using PHS.Networking.Enums;
using PHS.Networking.Handlers;
using PHS.Networking.Utilities;
using System;
using System.Linq;
using System.Net;
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
        protected bool _isRunning;

        public UdpClientHandlerBase(W parameters) : base(parameters)
        {
            _isRunning = true;
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

                    _isRunning = true;

                    CreateConnection();
                    
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
            }

            await DisconnectAsync(cancellationToken).ConfigureAwait(false);

            return false;
        }
        public override async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection != null)
                {
                    if (!_connection.Disposed)
                    {
                        _connection.Disposed = true;

                        if (_connection.Socket != null && _parameters.UseDisconnectBytes)
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

                        _isRunning = false;

                        return true;
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

            _isRunning = false;

            return false;
        }

        public override async Task<bool> SendAsync(string message, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection != null &&
                    _connection.Socket != null &&
                    _connection.Socket.Connected &&
                    !cancellationToken.IsCancellationRequested &&
                    !string.IsNullOrWhiteSpace(message))
                {
                    var bytes = Encoding.UTF8.GetBytes(_connection.ConnectionId).Concat(_parameters.PrefixTerminator).Concat(Encoding.UTF8.GetBytes(message)).ToArray();
                    _connection.Socket.Send(bytes, SocketFlags.None);

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
            }

            await DisconnectAsync(cancellationToken).ConfigureAwait(false);

            return false;
        }
        public override async Task<bool> SendAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_connection != null &&
                    _connection.Socket != null &&
                    _connection.Socket.Connected &&
                    !cancellationToken.IsCancellationRequested &&
                    message.Where(x => x != 0).Any())
                {
                    var bytes = Encoding.UTF8.GetBytes(_connection.ConnectionId).Concat(_parameters.PrefixTerminator).Concat(message).ToArray();
                    _connection.Socket.Send(bytes, SocketFlags.None);

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
            }

            await DisconnectAsync(cancellationToken).ConfigureAwait(false);

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
                    var result = _connection.Socket.Receive(buffer);

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

                            _isRunning = false;
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

        protected virtual void CreateConnection()
        {
            // Establish the remote endpoint for the socket.  
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 60000
            };

            socket.Connect(_parameters.Host, _parameters.Port);
            
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

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }
    }
}
