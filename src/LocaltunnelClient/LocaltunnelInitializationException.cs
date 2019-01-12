// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelInitializationException.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Runtime.Serialization;

namespace LocaltunnelClient
{
    [Serializable]
    public class LocaltunnelInitializationException : LocaltunnelException
    {
        /// <inheritdoc />
        protected LocaltunnelInitializationException(SerializationInfo info, StreamingContext context) : base(
            info, context) { }

        /// <inheritdoc />
        public LocaltunnelInitializationException(string message, Exception innerException) : base(
            message, innerException) { }
    }
}