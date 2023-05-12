using PHS.Networking.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Client;
using Udp.NET.Client.Events.Args;
using Udp.NET.Client.Models;

namespace Udp.NET.TestApps.Client
{
    class Program
    {
        private static List<IUdpNETClient> _clients = new List<IUdpNETClient>();
        private static Timer _timer;
        private static int _max;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter numbers of users per minute:");
            var line = Console.ReadLine();
            var numberUsers = 0;
            while (!int.TryParse(line, out numberUsers))
            {
                Console.WriteLine("Invalid. Input an int:");
                line = Console.ReadLine();
            }

            Console.WriteLine("Enter max number of users:");
            line = Console.ReadLine();
            _max = 0;
            while (!int.TryParse(line, out _max))
            {
                Console.WriteLine("Invalid. Input an int:");
                line = Console.ReadLine();
            }

            Console.WriteLine("Push any key to start");

            Console.ReadLine();

            _timer = new Timer(OnTimerTick, null, 0, CalculateNumberOfUsersPerMinute(numberUsers));

            while (true)
            {
                line = Console.ReadLine();

                if (line == "restart")
                {
                    foreach (var item in _clients.ToList())
                    {
                        if (item != null)
                        {
                            await item.DisconnectAsync();
                        }
                    }
                }
                else
                {
                    await _clients.ToList().Where(x => x != null && x.IsRunning).OrderBy(x => Guid.NewGuid()).First().SendAsync(line);
                }
            }
        }

        private static void OnTimerTick(object state)
        {
            if (_clients.Count < _max)
            {
                var client = new UdpNETClient(new ParamsUdpClient("localhost", 8989, token: "testToken"));
                client.ConnectionEvent += OnConnectionEvent;
                client.MessageEvent += OnMessageEvent;
                client.ErrorEvent += OnErrorEvent;
                _clients.Add(client);

                Task.Run(async () => await client.ConnectAsync());
            }
        }
        private static void OnErrorEvent(object sender, UdpErrorClientEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
        private static void OnConnectionEvent(object sender, UdpConnectionClientEventArgs args)
        {
            Console.WriteLine(args.ConnectionEventType);

            switch (args.ConnectionEventType)
            {
                case ConnectionEventType.Connected:
                    break;
                case ConnectionEventType.Disconnect:
                    var client = (IUdpNETClient)sender;
                    _clients.Remove(client);

                    client.ConnectionEvent -= OnConnectionEvent;
                    client.MessageEvent -= OnMessageEvent;
                    client.ErrorEvent -= OnErrorEvent;

                    client.Dispose();
                    break;
                default:
                    break;
            }
        }
        private static void OnMessageEvent(object sender, UdpMessageClientEventArgs args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    break;
                case MessageEventType.Receive:
                    Console.WriteLine(args.Message + " : " + +_clients.Where(x => x != null && x.IsRunning).Count());
                    break;
                default:
                    break;
            }
        }

        static int CalculateNumberOfUsersPerMinute(int numberUsers)
        {
            return 60000 / numberUsers;
        }
    }
}
