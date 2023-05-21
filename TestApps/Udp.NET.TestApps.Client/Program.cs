using PHS.Networking.Enums;
using System;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<int, IUdpNETClient> _clients = new ConcurrentDictionary<int, IUdpNETClient>();
        private static int _max;
        private static bool _isDone;

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

            _ = new Timer(OnTimerTick, null, 0, CalculateNumberOfUsersPerMinute(numberUsers));

            while (true)
            {
                line = Console.ReadLine();

                if (line == "restart")
                {
                    foreach (var item in _clients.Values.ToList())
                    {
                        if (item != null)
                        {
                            await item.DisconnectAsync();
                        }
                    }
                }
                else
                {
                    await _clients.Values.ToList().Where(x => x.IsRunning).OrderBy(x => Guid.NewGuid()).First().SendAsync(line);
                }
            }
        }

        private static void OnTimerTick(object state)
        {
            if (!_isDone && _clients.Values.Where(x => x.IsRunning).Count() < _max)
            {
                var client = new UdpNETClient(new ParamsUdpClient("localhost", 8989, token: "testToken"));
                client.ConnectionEvent += OnConnectionEvent;
                client.MessageEvent += OnMessageEvent;
                client.ErrorEvent += OnErrorEvent;
                _clients.TryAdd(client.GetHashCode(), client);

                Task.Run(async () => await client.ConnectAsync());

                if (_clients.Values.Where(x => x.IsRunning).Count() >= _max)
                {
                    _isDone = true;
                }
            }
        }
        private static void OnErrorEvent(object sender, UdpErrorClientEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
        private static void OnConnectionEvent(object sender, UdpConnectionClientEventArgs args)
        {
            switch (args.ConnectionEventType)
            {
                case ConnectionEventType.Connected:
                    break;
                case ConnectionEventType.Disconnect:
                    var client = (IUdpNETClient)sender;
                    _clients.TryRemove(_clients.FirstOrDefault(x => x.Key == client.GetHashCode()));

                    client.ConnectionEvent -= OnConnectionEvent;
                    client.MessageEvent -= OnMessageEvent;
                    client.ErrorEvent -= OnErrorEvent;

                    client.Dispose();
                    break;
                default:
                    break;
            }
            
            Console.WriteLine(args.ConnectionEventType + " " + _clients.Values.Where(x => x.IsRunning).Count());
        }
        private static void OnMessageEvent(object sender, UdpMessageClientEventArgs args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    break;
                case MessageEventType.Receive:
                    //Console.WriteLine(args.Message + " : " + +_clients.Where(x => x != null && x.IsRunning).Count());
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
