using PHS.Networking.Server.Services;
using System;
using System.Net.Sockets;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Handlers;
using Udp.NET.Server.Managers;
using Udp.NET.Server.Models;

namespace Udp.NET.Server
{
    public class UdpNETServerAuth<T> :
        UdpNETServerAuthBase<
            UdpConnectionServerAuthEventArgs<T>, 
            UdpMessageServerAuthEventArgs<T>, 
            UdpErrorServerAuthEventArgs<T>,
            ParamsUdpServerAuth,
            UdpHandlerServerAuth<T>,
            UdpConnectionManagerAuth<T>,
            IdentityUdpServer<T>,
            T>,
        IUdpNETServerAuth<T>
    {
        public UdpNETServerAuth(ParamsUdpServerAuth parameters,
            IUserService<T> userService) : base(parameters, userService)
        { 
        }

        protected override UdpConnectionManagerAuth<T> CreateConnectionManager()
        {
            return new UdpConnectionManagerAuth<T>();
        }
        protected override UdpHandlerServerAuth<T> CreateHandler()
        {
            return new UdpHandlerServerAuth<T>(_parameters);
        }

        protected override UdpConnectionServerAuthEventArgs<T> CreateConnectionEventArgs(UdpConnectionServerBaseEventArgs<IdentityUdpServer<T>> args)
        {
            return new UdpConnectionServerAuthEventArgs<T>
            {
                Connection = args.Connection,
                ConnectionEventType = args.ConnectionEventType,
                CancellationToken = args.CancellationToken
            };
        }
        protected override UdpMessageServerAuthEventArgs<T> CreateMessageEventArgs(UdpMessageServerBaseEventArgs<IdentityUdpServer<T>> args)
        {
            return new UdpMessageServerAuthEventArgs<T>
            {
                Bytes = args.Bytes,
                Connection = args.Connection,
                Message = args.Message,
                MessageEventType = args.MessageEventType,
                CancellationToken = args.CancellationToken
            };
        }
        protected override UdpErrorServerAuthEventArgs<T> CreateErrorEventArgs(UdpErrorServerBaseEventArgs<IdentityUdpServer<T>> args)
        {
            return new UdpErrorServerAuthEventArgs<T>
            {
                Connection = args.Connection,
                Exception = args.Exception,
                Message = args.Message,
                CancellationToken = args.CancellationToken
            };
        }
        protected override IdentityUdpServer<T> CreateConnection(UdpReceiveResult udpReceiveResult)
        {
            return new IdentityUdpServer<T>
            {
                ConnectionId = Guid.NewGuid().ToString(),
                IpEndpoint = udpReceiveResult.RemoteEndPoint
            };
        }
    }
}
