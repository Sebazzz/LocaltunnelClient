// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelRemoteConnectedEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System.Net;

namespace LocaltunnelClient.Events
{
    public sealed class TunnelRemoteConnectedEvent : TunnelConnectedEvent
    {
        /// <inheritdoc />
        public TunnelRemoteConnectedEvent(int tunnelId, EndPoint remoteEndPoint, EndPoint localEndPoint) : base(MakeMessage(remoteEndPoint, localEndPoint), tunnelId, remoteEndPoint, localEndPoint)
        {
        }

        private static string MakeMessage(EndPoint remoteEndPoint, EndPoint localEndPoint)
        {
            return $"Connected to remotely {localEndPoint} to {remoteEndPoint}";
        }
    }
}