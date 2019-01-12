// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

namespace LocaltunnelClient.Events
{
    public abstract class LocaltunnelEvent
    {
        public string Message { get; }

        /// <inheritdoc />
        protected LocaltunnelEvent(string message)
        {
            this.Message = message;
        }
    }
}