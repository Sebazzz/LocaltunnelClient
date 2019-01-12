// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelClosedEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

namespace LocaltunnelClient.Events
{
    public sealed class TunnelClosedEvent : TunnelOriginatedEvent
    {
        /// <inheritdoc />
        public TunnelClosedEvent(int tunnelId) : base("Tunnel closed", tunnelId) { }
    }
}