// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : ConsoleLocaltunnelRunner.cs
//  Project         : LocaltunnelClient.Commandline
// ******************************************************************************

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LocaltunnelClient.Events;

namespace LocaltunnelClient.Commandline.Internal
{
    internal sealed class ConsoleLocaltunnelRunner : IDisposable
    {
        private readonly Localtunnel _localtunnel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ConsoleCancelEventHandler _cancelEventHandler;
        private readonly IProgress<LocaltunnelEvent> _progressSink;

        /// <inheritdoc />
        public ConsoleLocaltunnelRunner(LocaltunnelOptions localtunnelOptions, bool openInWebBrowser, bool verbose)
        {
            this._localtunnel = new Localtunnel(localtunnelOptions);
            this._cancellationTokenSource = new CancellationTokenSource();

            this._progressSink = new InternalProgressSink(openInWebBrowser, verbose, localtunnelOptions.Server, this._cancellationTokenSource.Token);

            this._cancelEventHandler = (sender, args) =>
            {
                Console.WriteLine("Shutting down...");

                args.Cancel = true;
                this._cancellationTokenSource.Cancel();
            };

            Console.CancelKeyPress += this._cancelEventHandler;
        }

        public async Task RunAsync()
        {
            await this._localtunnel.RunAsync(this._progressSink, this._cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            Console.CancelKeyPress -= this._cancelEventHandler;
            this._cancellationTokenSource?.Dispose();
        }

        private sealed class InternalProgressSink : IProgress<LocaltunnelEvent>
        {
            private readonly bool _openInWebBrowser;
            private readonly bool _verbose;
            private readonly string _localTunnelServer;
            private readonly CancellationToken _token;

            /// <inheritdoc />
            public InternalProgressSink(bool openInWebBrowser, bool verbose, string localTunnelServer,
                                        CancellationToken token)
            {
                this._openInWebBrowser = openInWebBrowser;
                this._verbose = verbose;
                this._localTunnelServer = localTunnelServer;
                this._token = token;
            }


            /// <inheritdoc />
            public void Report(LocaltunnelEvent value)
            {
                if (value is HostAssignedEvent hostAssignedEvent && this._openInWebBrowser)
                {
                    // Wait a little bit with starting the web browser
                    Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None).ContinueWith(_ =>
                    {
                        if (this._token.IsCancellationRequested)
                        {
                            return;
                        }

                        UriBuilder uri = new UriBuilder(this._localTunnelServer);
                        uri.Host = hostAssignedEvent.Info.HostName + "." + uri.Host;
                        uri.Query = null;
                        uri.Path = null;
                        uri.Fragment = null;

                        Process.Start(uri.ToString());
                    }, CancellationToken.None);
                }

                if (value is TunnelOriginatedEvent && !(value is TunnelClosedEvent) && this._verbose == false)
                {
                    // Skip tunnel originated events if verbose == false
                    return;
                }

                Console.WriteLine(value.Message);
            }
        }
    }
}