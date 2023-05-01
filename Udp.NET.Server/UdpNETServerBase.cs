using Udp.NET.Server.Models;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Server.Handlers;
using Udp.NET.Server.Managers;
using PHS.Networking.Server.Events.Args;
using Udp.NET.Server.Events.Args;
using PHS.Networking.Server.Enums;
using PHS.Networking.Server.Services;
using System;
using PHS.Networking.Enums;
using Udp.NET.Server.Events.Args;
using PHS.Networking.Server.Managers;

namespace Udp.NET.Server
{
    public abstract class UdpNETServerBase<T, U, V, W, X, Y, Z> : 
        CoreNetworkingServer<T, U, V, W, X, Y, Z>, 
        ICoreNetworkingServer<T, U, V, Z>
        where T : UdpConnectionServerBaseEventArgs<Z>
        where U : UdpMessageServerBaseEventArgs<Z>
        where V : UdpErrorServerBaseEventArgs<Z>
        where W : ParamsUdpServer
        where X : UdpHandlerServerBase<T, U, V, W, Z>
        where Y : ConnectionManager<Z>
        where Z : ConnectionUdpServer
    {
        protected Timer _timerPing;
        protected bool _isPingRunning;
        
        public UdpNETServerBase(W parameters) : base(parameters)
        {
            _handler.ReceivedEvent += OnReceivedEvent;
        }

        protected virtual void OnReceivedEvent(object sender, UdpReceivedEventArgs args)
        {
            if (!_connectionManager.GetConnection(args.UdpReceiveResult.RemoteEndPoint.Serialize().ToString(), out var connection))
            {
                connection = CreateConnection(args.UdpReceiveResult);
                _connectionManager.AddConnection(connection.IpEndpoint.Serialize().ToString(), connection);
            }

            _handler.Receive(args.UdpReceiveResult.Buffer, connection, args.CancellationToken);
        }
        protected override void OnServerEvent(object sender, ServerEventArgs args)
        {
            if (_timerPing != null)
            {
                _timerPing.Dispose();
                _timerPing = null;
            }

            if (_parameters.PingIntervalSec > 0)
            {
                switch (args.ServerEventType)
                {
                    case ServerEventType.Start:
                        _timerPing = new Timer(OnTimerPingTick, args.CancellationToken, _parameters.PingIntervalSec * 1000, _parameters.PingIntervalSec * 1000);
                        break;
                    case ServerEventType.Stop:
                        break;
                    default:
                        break;
                }
            }

            base.FireEvent(sender, args);
        }
        protected virtual void OnTimerPingTick(object state)
        {
            if (!_isPingRunning)
            {
                _isPingRunning = true;

                Task.Run(async () =>
                {
                    foreach (var connection in _connectionManager.GetAllConnections())
                    {
                        try
                        {
                            if (connection.HasBeenPinged)
                            {
                                await SendToConnectionAsync("No ping response - disconnected.", connection, (CancellationToken)state).ConfigureAwait(false);
                                await DisconnectConnectionAsync(connection, (CancellationToken)state).ConfigureAwait(false);
                            }
                            else
                            {
                                connection.HasBeenPinged = true;
                                await SendToConnectionAsync(_parameters.PingBytes, connection, (CancellationToken)state).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            FireEvent(this, CreateErrorEventArgs(new UdpErrorServerBaseEventArgs<Z>
                            {
                                Connection = connection,
                                Exception = ex,
                                Message = ex.Message
                            }));
                        }
                    }

                    _isPingRunning = false;
                });
            }
        }

        protected abstract Z CreateConnection(UdpReceiveResult udpReceiveResult);
        protected abstract T CreateConnectionEventArgs(UdpConnectionServerBaseEventArgs<Z> args);
        protected abstract U CreateMessageEventArgs(UdpMessageServerBaseEventArgs<Z> args);
        protected abstract V CreateErrorEventArgs(UdpErrorServerBaseEventArgs<Z> args);

        public override void Dispose()
        {
            if (_handler != null)
            {
                _handler.ReceivedEvent -= OnReceivedEvent;
            }

            if (_timerPing != null)
            {
                _timerPing.Dispose();
                _timerPing = null;
            }

            base.Dispose();
        }

        public UdpClient Server
        {
            get
            {
                return _handler?.Server;
            }
        }
    }
}
