using PHS.Networking.Enums;
using PHS.Networking.Events.Args;
using System.Net;
using Udp.NET.Server.Events.Args;
using Udp.NET.Server.Models;

namespace Udp.NET.Server.Handlers
{
    public class UdpHandlerServer : 
        UdpHandlerServerBase<
            UdpConnectionServerEventArgs,
            UdpMessageServerEventArgs,
            UdpErrorServerEventArgs,
            ParamsUdpServer,
            ConnectionUdpServer>
    {
        public UdpHandlerServer(ParamsUdpServer parameters) : base(parameters)
        {
        }

        protected override UdpConnectionServerEventArgs CreateConnectionEventArgs(ConnectionEventArgs<ConnectionUdpServer> args)
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
                Connection = args.Connection,
                Bytes = args.Bytes,
                Message = args.Message,
                MessageEventType = args.MessageEventType,
                CancellationToken = args.CancellationToken
            };
        }
        protected override UdpErrorServerEventArgs CreateErrorEventArgs(ErrorEventArgs<ConnectionUdpServer> args)
        {
            return new UdpErrorServerEventArgs
            {
                Connection = args.Connection,
                Exception = args.Exception,
                Message = args.Message,
                CancellationToken = args.CancellationToken
            };
        }
    }
}
