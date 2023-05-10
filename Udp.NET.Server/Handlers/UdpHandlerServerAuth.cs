using PHS.Networking.Enums;
using PHS.Networking.Events.Args;
using PHS.Networking.Utilities;
using System.Text;
using System.Threading;
using System;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Models;
using System.Linq;

namespace Udp.NET.Server.Handlers
{
    public class UdpHandlerServerAuth<T> : 
        UdpHandlerServerBase<
            UdpConnectionServerAuthEventArgs<T>,
            UdpMessageServerAuthEventArgs<T>,
            UdpErrorServerAuthEventArgs<T>,
            ParamsUdpServerAuth,
            IdentityUdpServer<T>>
    {
        public UdpHandlerServerAuth(ParamsUdpServerAuth parameters) : base(parameters)
        {
        }

        protected override UdpConnectionServerAuthEventArgs<T> CreateConnectionEventArgs(ConnectionEventArgs<IdentityUdpServer<T>> args)
        {
            return new UdpConnectionServerAuthEventArgs<T>
            {
                Connection = args.Connection,
                ConnectionEventType = args.ConnectionEventType,
                CancellationToken = args.CancellationToken
            };
        }
        protected override UdpErrorServerAuthEventArgs<T> CreateErrorEventArgs(ErrorEventArgs<IdentityUdpServer<T>> args)
        {
            return new UdpErrorServerAuthEventArgs<T>
            {
                Connection = args.Connection,
                Exception = args.Exception,
                Message = args.Message,
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
    }
}
