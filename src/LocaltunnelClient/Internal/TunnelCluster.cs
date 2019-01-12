// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelCluster.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocaltunnelClient.Events;

namespace LocaltunnelClient.Internal
{
    internal sealed class TunnelCluster : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IProgress<LocaltunnelEvent> _progress;
        
        private readonly string _localtunnelServer;
        private readonly int _localPort;
        private readonly TunnelServerResponse _tunnelServerInfo;

        private readonly Tunnel[] _tunnels;
        private CancellationTokenRegistration _cancellationToken;

        /// <inheritdoc />
        public TunnelCluster(IProgress<LocaltunnelEvent> progress, CancellationToken cancellationToken,
                             LocaltunnelOptions tunnelOptions, TunnelServerResponse tunnelServerInfo)
        {
            this._progress = progress;
            this._cancellationToken = cancellationToken.Register(() => this._cancellationTokenSource.Cancel());
            this._localtunnelServer = tunnelOptions.GetHostName();
            this._localPort = tunnelOptions.LocalPort;
            this._tunnelServerInfo = tunnelServerInfo;
            this._cancellationTokenSource = new CancellationTokenSource();
            ;

            this._tunnels = new Tunnel[tunnelServerInfo.MaxAllowedConnections];
            for (int index = 0; index < this._tunnels.Length; index++)
            {
                this._tunnels[index] = this.CreateTunnel(index);
            }
        }

        public async Task RunAsync()
        {
            Task[] runTasks = new Task[this._tunnels.Length];
            for (int index = 0; index < this._tunnels.Length; index++)
            {
                runTasks[index] = this.RunTunnelAsync(index);
            }

            try
            {
                await Task.WhenAll(runTasks);
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Any(x => x is LocaltunnelTunnelConnectionException))
            {
                throw new LocaltunnelTunnelConnectionException(
                    $"Unable to connect to remote tunnel {this._localtunnelServer}:{this._tunnelServerInfo.RemotePort}",
                    ex);
            }
            catch (Exception ex) when (ex is OperationCanceledException ||
                                       this._cancellationTokenSource.IsCancellationRequested)
            {
                this._cancellationTokenSource.Token.ThrowIfCancellationRequested();
                throw;
            }
            finally
            {
                this._cancellationToken.Dispose();
            }
        }

        private async Task RunTunnelAsync(int tunnelIndex)
        {
            Tunnel tunnel = this._tunnels[tunnelIndex];

            while (!this._cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    await tunnel.RunAsync(this._progress, this._cancellationTokenSource.Token);
                
                    this._cancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                finally
                {
                    tunnel.Dispose();
                }

                this._progress.Report(new TunnelClosedEvent(tunnelIndex));

                this._tunnels[tunnelIndex] = tunnel = this.CreateTunnel(tunnelIndex);
            }

            tunnel.Dispose();
        }

        private Tunnel CreateTunnel(int index)
        {
            return new Tunnel(index, this._localtunnelServer, this._tunnelServerInfo.RemotePort, this._localPort);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (Tunnel tunnel in this._tunnels)
            {
                tunnel.Dispose();
            }

            this._cancellationTokenSource.Dispose();

            this._cancellationToken.Dispose();
        }
    }
}