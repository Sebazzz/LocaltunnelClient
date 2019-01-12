// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : Tunnel.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LocaltunnelClient.Events;

namespace LocaltunnelClient.Internal
{
    internal sealed class Tunnel : IDisposable
    {
        private const int BufferSize = 1024;

        private readonly string _host;
        private readonly int _port;
        private readonly int _localPort;
        private readonly int _id;

        private TcpClient _tcpClient;

        /// <inheritdoc />
        public Tunnel(int id, string host, int port, int localPort)
        {
            this._host = host;
            this._port = port;
            this._localPort = localPort;
            this._id = id;
            this._tcpClient = new TcpClient();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._tcpClient?.Dispose();
            this._tcpClient = null;
        }

        /// <summary>
        /// Runs the current tunnel until it is canceled or an error occurred
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RunAsync(IProgress<LocaltunnelEvent> progress, CancellationToken cancellationToken)
        {
            this._tcpClient.NoDelay = true;
            this._tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // Connect to remote
            try
            {
                // Even though this task isn't cancellable, and WithCancellation will 
                // not prevent the task continuing, it does allow to execute immediate cancellation
                await this._tcpClient.ConnectAsync(this._host, this._port).WithCancellation(cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex) when (ex.GetBaseException() is SocketException)
            {
                this._tcpClient.Close();

                SocketException socketException = ((SocketException) ex.GetBaseException());
                SocketError errorCode = socketException.SocketErrorCode;

                throw new LocaltunnelTunnelConnectionException($"Unable to connect - error code {errorCode} - 0x{socketException.HResult:x8}",
                                                               ex);
            }
            catch (OperationCanceledException)
            {
                this._tcpClient.Close();

                throw;
            }
            catch (Exception ex)
            {
                this._tcpClient.Close();

                throw new LocaltunnelTunnelConnectionException($"Unable to connect - error code 0x{ex.HResult:x8}",
                                                               ex);
            }
            
            progress.Report(new TunnelRemoteConnectedEvent(this._id, this._tcpClient.Client.RemoteEndPoint, this._tcpClient.Client.LocalEndPoint));

            // Open remoteStream to read data, once we do get data, we will connect the local and pipe the streams
            using (NetworkStream stream = this._tcpClient.GetStream())
            {
                await this.WaitForDataAndPipeStreamAsync(stream, progress, cancellationToken);

                // If no exception happened, then at this point the local stream
                // is closed. We could reuse the remote stream, but we might as well
                // create an entire new connection. 
            }
        }

        private async Task WaitForDataAndPipeStreamAsync(NetworkStream remoteStream, IProgress<LocaltunnelEvent> progress, CancellationToken cancellationToken)
        {
            // Wait for some data to come through
            // While not strictly the case in a regular TCP tunnel, in HTTP a remote would initiate
            byte[] readBuffer = new byte[BufferSize];
            int readBytes;
            try
            {
                readBytes = await remoteStream.ReadAsync(readBuffer, 0, readBuffer.Length, cancellationToken);
            }
            catch (Exception ex)
            {
                progress.Report(new TunnelDeadEvent(this._id, ex));
                return;
            }

            progress.Report(new TunnelInitialDataEvent(this._id, readBytes));

            // Connect to local
            using (TcpClient localTcpClient = await this.ConnectLocallyAsync(progress, cancellationToken))
            {
                using (NetworkStream localStream = localTcpClient.GetStream())
                {
                    // Write the initially read buffer
                    await localStream.WriteAsync(readBuffer, 0, readBytes, cancellationToken);

                    // Pipe the streams together
                    await this.PipeStreamsAsync(remoteStream, localStream, progress, cancellationToken);
                }
            }
        }

        private async Task PipeStreamsAsync(NetworkStream remoteStream, NetworkStream localStream, IProgress<LocaltunnelEvent> progress, CancellationToken cancellationToken)
        {
            Task<SocketException> remoteToLocal = Task.Run(() => this.PipeStreamAsync(remoteStream, localStream, cancellationToken), cancellationToken);
            Task<SocketException> localToRemote = Task.Run(() => this.PipeStreamAsync(localStream, remoteStream, cancellationToken), cancellationToken);

            try
            {
                Exception[] result = await Task.WhenAll(remoteToLocal, localToRemote);

                progress.Report(new TunnelDeadEvent(this._id, new AggregateException(result)));
            }
            catch (Exception ex) when (ex.GetBaseException() is OperationCanceledException)
            {
                throw new OperationCanceledException("Cancellation requested", ex, cancellationToken);
            }
        }

        private async Task<SocketException> PipeStreamAsync(NetworkStream fromStream, NetworkStream toStream, CancellationToken cancellationToken)
        {
            // Read from the source stream, write to the destination stream
            while (true)
            {
                byte[] buffer = new byte[BufferSize];

                while (!fromStream.DataAvailable)
                {
                    await Task.Yield();
                }

                repeatRead: 
                int readBytes;
                try
                {
                    readBytes = await fromStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                }
                catch (Exception ex) when (!(ex.GetBaseException() is OperationCanceledException))
                {
                    // Allow the target stream to process the data before fully cancelling
                    await toStream.FlushAsync(cancellationToken);

                    if (ex.GetBaseException() is SocketException socketException)
                    {
                        switch (socketException.SocketErrorCode)
                        {
                            case SocketError.ConnectionAborted:
                            case SocketError.ConnectionReset:
                            case SocketError.Disconnecting:
                            case SocketError.NetworkDown:
                            case SocketError.NetworkReset:
                            case SocketError.OperationAborted:
                                return socketException;

                            case SocketError.TimedOut:
                                goto repeatRead;

                            default:
                                throw;
                        }
                    }

                    throw;
                }

                await toStream.WriteAsync(buffer, 0, readBytes, cancellationToken);
            }
        }

        private async Task<TcpClient> ConnectLocallyAsync(IProgress<LocaltunnelEvent> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TcpClient localTcpClient = new TcpClient
            {
                NoDelay = true
            };

            try
            {
                // Even though this task isn't cancellable, and WithCancellation will 
                // not prevent the task continuing, it does allow to execute immediate cancellation
                await localTcpClient.ConnectAsync(IPAddress.Loopback, this._localPort)
                                    .WithCancellation(cancellationToken);
            }
            catch (Exception ex)
            {
                localTcpClient.Dispose();
                
                progress.Report(new TunnelLocalConnectionErrorEvent(this._id, ex));

                throw new OperationCanceledException($"Unable to connect locally: {ex.Message}", ex, cancellationToken);
            }

            progress.Report(new TunnelLocalConnectedEvent(this._id, localTcpClient.Client.RemoteEndPoint, localTcpClient.Client.LocalEndPoint));

            return localTcpClient;
        }
    }
}