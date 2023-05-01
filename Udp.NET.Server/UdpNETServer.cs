using Udp.NET.Server.Models;
using Udp.NET.Server.Handlers;
using Udp.NET.Server.Managers;
using Udp.NET.Server.Events.Args;
using System.Net.Sockets;
using System;

namespace Udp.NET.Server
{
    public class UdpNETServer :
        UdpNETServerBase<
            UdpConnectionServerEventArgs,
            UdpMessageServerEventArgs,
            UdpErrorServerEventArgs,
            ParamsUdpServer,
            UdpHandlerServer,
            UdpConnectionManager,
            ConnectionUdpServer>,
        IUdpNETServer
    {
        public UdpNETServer(ParamsUdpServer parameters) : base(parameters)
        {
        }

        protected override UdpConnectionManager CreateConnectionManager()
        {
            return new UdpConnectionManager();
        }

        protected override UdpHandlerServer CreateHandler(byte[] certificate = null, string certificatePassword = null)
        {
            return new UdpHandlerServer(_parameters);
        }

        protected override UdpErrorServerEventArgs CreateErrorEventArgs(UdpErrorServerBaseEventArgs<ConnectionUdpServer> args)
        {
            return new UdpErrorServerEventArgs
            {
                Connection = args.Connection,
                Exception = args.Exception,
                Message = args.Message,
                CancellationToken = args.CancellationToken
            };
        }

        protected override UdpConnectionServerEventArgs CreateConnectionEventArgs(UdpConnectionServerBaseEventArgs<ConnectionUdpServer> args)
        {
            return new UdpConnectionServerEventArgs
            {
                Connection = args.Connection,
                ConnectionEventType = args.ConnectionEventType,
                CancellationToken = args.CancellationToken
            };
        }

        protected override UdpMessageServerEventArgs CreateMessageEventArgs(UdpMessageServerBaseEventArgs<ConnectionUdpServer> args)
        {
            return new UdpMessageServerEventArgs
            {
                Bytes = args.Bytes,
                Connection = args.Connection,
                Message = args.Message,
                MessageEventType = args.MessageEventType,
                CancellationToken = args.CancellationToken
            };
        }

        protected override ConnectionUdpServer CreateConnection(UdpReceiveResult udpReceiveResult)
        {
            return new ConnectionUdpServer
            {
                ConnectionId = Guid.NewGuid().ToString(),
                IpEndpoint = udpReceiveResult.RemoteEndPoint
            };
        }
    }
}
