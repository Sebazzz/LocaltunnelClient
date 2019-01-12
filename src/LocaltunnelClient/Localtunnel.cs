// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : Localtunnel.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Threading;
using System.Threading.Tasks;
using LocaltunnelClient.Events;
using LocaltunnelClient.Internal;

namespace LocaltunnelClient
{
    public sealed class Localtunnel
    {
        private readonly LocaltunnelOptions _options;

        public LocaltunnelState State { get; private set; } = LocaltunnelState.NotStarted;

        public LocaltunnelInfo RunningInfo { get; private set; }

        /// <inheritdoc />
        /// <see cref="LocaltunnelOptionsValidationException"/>
        public Localtunnel(LocaltunnelOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            this._options = options.Clone();
            this._options.Validate();
        }

        public async Task RunAsync(IProgress<LocaltunnelEvent> progress, CancellationToken cancellationToken)
        {
            if (this.State == LocaltunnelState.Active)
            {
                return;
            }

            if (this.State != LocaltunnelState.NotStarted)
            {
                throw new InvalidOperationException($"The tunnel is cannot be started (State: {this.State})");
            }

            this.State = LocaltunnelState.Starting;

            TunnelServerResponse tunnelServerResponse;
            try
            {
                tunnelServerResponse = await ServerApi.RequestTunnelAsync(this._options.Server, this._options.Subdomain, cancellationToken);
            }
            catch (Exception)
            {
                this.State = LocaltunnelState.NotStarted;
                throw;
            }

            this.RunningInfo = new LocaltunnelInfo(tunnelServerResponse);

            progress.Report(new HostAssignedEvent(
                $"Local tunnel address {tunnelServerResponse.Id}.{this._options.GetHostName()} was assigned",
                this.RunningInfo));

            TunnelCluster cluster = null;
            try
            {
                cluster = new TunnelCluster(progress, cancellationToken, this._options, tunnelServerResponse);

                this.State = LocaltunnelState.Active;

                await cluster.RunAsync();
            }
            catch (Exception)
            {
                this.RunningInfo = null;

                throw;
            }
            finally
            {
                cluster?.Dispose();

                this.State = LocaltunnelState.NotStarted;
            }
        }
        
    }
}