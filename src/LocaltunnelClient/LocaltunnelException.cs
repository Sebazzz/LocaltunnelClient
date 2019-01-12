// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelException.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Runtime.Serialization;

namespace LocaltunnelClient
{
    [Serializable]
    public class LocaltunnelException : Exception
    {
        /// <inheritdoc />
        public LocaltunnelException() { }

        /// <inheritdoc />
        protected LocaltunnelException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <inheritdoc />
        public LocaltunnelException(string message) : base(message) { }

        /// <inheritdoc />
        public LocaltunnelException(string message, Exception innerException) : base(message, innerException) { }
    }
}