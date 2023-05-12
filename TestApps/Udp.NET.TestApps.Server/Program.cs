using PHS.Networking.Server.Events.Args;
using System;
using System.Threading.Tasks;
using Udp.NET.Server;
using Udp.NET.Server.Models;
using Udp.NET.Server.Events.Args;
using PHS.Networking.Enums;

namespace Udp.NET.TestApps.Server
{
    class Program
    {
        private static IUdpNETServerAuth<Guid> _authServer;

        static async Task Main(string[] args)
        {
            _authServer = new UdpNETServerAuth<Guid>(new ParamsUdpServerAuth(8989, "Connected Successfully", "Not authorized"), new MockUserService()); ;
            _authServer.MessageEvent += OnMessageEvent;
            _authServer.ServerEvent += OnServerEvent;
            _authServer.ConnectionEvent += OnConnectionEvent;
            _authServer.ErrorEvent += OnErrorEvent;
            _authServer.Start();

            while (true)
            {
                var line = Console.ReadLine();

                foreach (var item in _authServer.Connections)
                {
                    await _authServer.DisconnectConnectionAsync(item);
                }
            }
        }

        private static void OnErrorEvent(object sender, UdpErrorServerAuthEventArgs<Guid> args)
        {
            Console.WriteLine(args.Message);
        }

        private static void OnConnectionEvent(object sender, UdpConnectionServerAuthEventArgs<Guid> args)
        {
            Console.WriteLine(args.ConnectionEventType + " " + _authServer.ConnectionCount);
        }

        private static void OnServerEvent(object sender, ServerEventArgs args)
        {
            Console.WriteLine(args.ServerEventType);
        }

        private static void OnMessageEvent(object sender, UdpMessageServerAuthEventArgs<Guid> args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    break;
                case MessageEventType.Receive:
                    Console.WriteLine(args.MessageEventType + ": " + args.Message);

                    Task.Run(async () =>
                    {
                        Console.WriteLine("Connections: " + _authServer.ConnectionCount);
                        await _authServer.BroadcastToAllConnectionsAsync(args.Bytes);
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
