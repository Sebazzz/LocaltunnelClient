// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelConnectedEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System.Net;

namespace LocaltunnelClient.Events
{
    public abstract class TunnelConnectedEvent : TunnelOriginatedEvent
    {
        /// <inheritdoc />
        protected TunnelConnectedEvent(string message, int tunnelId, EndPoint remoteEndPoint, EndPoint localEndPoint) :
            base(message, tunnelId)
        {
            this.RemoteEndPoint = remoteEndPoint;
            this.LocalEndPoint = localEndPoint;
        }

        public EndPoint RemoteEndPoint { get; }

        public EndPoint LocalEndPoint { get; }
    }
}