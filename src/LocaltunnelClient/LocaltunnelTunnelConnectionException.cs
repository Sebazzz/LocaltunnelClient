// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelTunnelConnectionException.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Runtime.Serialization;

namespace LocaltunnelClient
{
    /// <summary>
    /// Occurs when the tunnel to the local tunnel server cannot be established
    /// </summary>
    [Serializable]
    public class LocaltunnelTunnelConnectionException : LocaltunnelException
    {
        /// <inheritdoc />
        public LocaltunnelTunnelConnectionException() { }

        /// <inheritdoc />
        protected LocaltunnelTunnelConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <inheritdoc />
        public LocaltunnelTunnelConnectionException(string message) : base(message) { }

        /// <inheritdoc />
        public LocaltunnelTunnelConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}