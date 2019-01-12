// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelOriginatedEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

namespace LocaltunnelClient.Events
{
    public abstract class TunnelOriginatedEvent : LocaltunnelEvent
    {
        /// <inheritdoc />
        protected TunnelOriginatedEvent(string message, int tunnelId) : base(
            $"Tunnel #{tunnelId,-1}: {message}")
        {
            this.TunnelId = tunnelId;
        }

        public int TunnelId { get; }
    }
}