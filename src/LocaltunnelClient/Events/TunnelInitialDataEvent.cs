// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelInitialDataEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

namespace LocaltunnelClient.Events
{
    public sealed class TunnelInitialDataEvent : TunnelOriginatedEvent
    {
        public int BytesReceived { get; set; }

        /// <inheritdoc />
        public TunnelInitialDataEvent(int tunnelId, int bytesReceived) : base(MakeMessage(bytesReceived), tunnelId)
        {
            this.BytesReceived = bytesReceived;
        }

        private static string MakeMessage(int bytesReceived)
        {
            return $"{bytesReceived}B of initial data received - connecting to local";
        }
    }
}