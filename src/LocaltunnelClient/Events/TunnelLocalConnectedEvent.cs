// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelLocalConnectedEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System.Net;

namespace LocaltunnelClient.Events
{
    public sealed class TunnelLocalConnectedEvent : TunnelConnectedEvent
    {
        /// <inheritdoc />
        public TunnelLocalConnectedEvent(int tunnelId, EndPoint remoteEndPoint, EndPoint localEndPoint) : base(
            MakeMessage(remoteEndPoint, localEndPoint), tunnelId, remoteEndPoint, localEndPoint) { }

        private static string MakeMessage(EndPoint remoteEndPoint, EndPoint localEndPoint)
        {
            return $"Connected locally {localEndPoint} to {remoteEndPoint}";
        }
    }
}