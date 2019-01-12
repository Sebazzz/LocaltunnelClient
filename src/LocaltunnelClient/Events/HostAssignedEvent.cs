// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : HostAssignedEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

namespace LocaltunnelClient.Events
{
    /// <summary>
    /// A tunnel address was assigned by the localtunnel server
    /// </summary>
    public sealed class HostAssignedEvent : LocaltunnelEvent
    {
        /// <inheritdoc />
        public HostAssignedEvent(string message, LocaltunnelInfo info) : base(message)
        {
            this.Info = info;
        }

        public LocaltunnelInfo Info { get; }
    }
}