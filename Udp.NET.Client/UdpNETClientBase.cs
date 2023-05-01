using PHS.Networking.Services;
using System.Threading;
using System.Threading.Tasks;
using Udp.NET.Client.Handlers;
using Udp.NET.Client.Models;
using Udp.NET.Core.Events.Args;
using Udp.NET.Core.Models;
using Udp.NET.Client.Models;

namespace Udp.NET.Client
{
    public abstract class UdpNETClientBase<T, U, V, W, X, Y> : 
        CoreNetworkingGeneric<T, U, V, W, Y>,
        ICoreNetworkingClient<T, U, V, Y>
        where T : UdpConnectionEventArgs<Y>
        where U : UdpMessageEventArgs<Y>
        where V : UdpErrorEventArgs<Y>
        where W : ParamsUdpClient
        where X : UdpClientHandlerBase<T, U, V, W, Y>
        where Y : ConnectionUdpClient
    {
        protected readonly X _handler;

        public UdpNETClientBase(W parameters) : base(parameters)
        {
            _handler = CreateTcpClientHandler();
            _handler.ConnectionEvent += OnConnectionEvent;
            _handler.MessageEvent += OnMessageEvent;
            _handler.ErrorEvent += OnErrorEvent;
        }

        public virtual async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            return await _handler.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }
        public virtual async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
        {
            return await _handler.DisconnectAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<bool> SendAsync(string message, CancellationToken cancellationToken = default)
        {
            return await _handler.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }
        public virtual async Task<bool> SendAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            return await _handler.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }

        protected virtual void OnConnectionEvent(object sender, T args)
        {
            FireEvent(this, args);
        }
        protected virtual void OnMessageEvent(object sender, U args)
        {
            FireEvent(this, args);
        }
        protected virtual void OnErrorEvent(object sender, V args)
        {
            FireEvent(this, args);
        }

        protected abstract X CreateTcpClientHandler();

        public override void Dispose()
        {
            if (_handler != null)
            {
                _handler.ConnectionEvent -= OnConnectionEvent;
                _handler.MessageEvent -= OnMessageEvent;
                _handler.ErrorEvent -= OnErrorEvent;
                _handler.Dispose();
            }
        }

        public bool IsRunning
        {
            get
            {
                return _handler?.Connection?.Socket?.Connected ?? false;
            }
        }
        public Y Connection
        {
            get
            {
                return _handler.Connection;
            }
        }
    }
}
