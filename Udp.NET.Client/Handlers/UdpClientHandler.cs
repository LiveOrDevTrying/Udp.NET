using Udp.NET.Client.Events.Args;
using Udp.NET.Client.Models;
using Udp.NET.Core.Events.Args;

namespace Udp.NET.Client.Handlers
{
    public class UdpClientHandler : 
        UdpClientHandlerBase<
            UdpConnectionClientEventArgs,
            UdpMessageClientEventArgs,
            UdpErrorClientEventArgs,
            ParamsUdpClient,
            ConnectionUdpClient>
    {
        public UdpClientHandler(ParamsUdpClient parameters) : base(parameters)
        {
        }

        protected override ConnectionUdpClient CreateConnection(ConnectionUdpClient connection)
        {
            return connection;
        }

        protected override UdpConnectionClientEventArgs CreateConnectionEventArgs(UdpConnectionEventArgs<ConnectionUdpClient> args)
        {
            return new UdpConnectionClientEventArgs
            {
                Connection = args.Connection,
                ConnectionEventType = args.ConnectionEventType,
                CancellationToken = args.CancellationToken
            };
        }

        protected override UdpErrorClientEventArgs CreateErrorEventArgs(UdpErrorEventArgs<ConnectionUdpClient> args)
        {
            return new UdpErrorClientEventArgs
            {
                Connection = args.Connection,
                Exception = args.Exception,
                Message = args.Message,
                CancellationToken = args.CancellationToken
            };
        }

        protected override UdpMessageClientEventArgs CreateMessageEventArgs(UdpMessageEventArgs<ConnectionUdpClient> args)
        {
            return new UdpMessageClientEventArgs
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
