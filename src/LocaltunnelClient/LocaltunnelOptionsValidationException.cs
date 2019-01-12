// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelOptionsValidationException.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Runtime.Serialization;

namespace LocaltunnelClient
{
    [Serializable]
    public class LocaltunnelOptionsValidationException : Exception
    {
        /// <inheritdoc />
        protected LocaltunnelOptionsValidationException(SerializationInfo info, StreamingContext context) : base(
            info, context) { }

        /// <inheritdoc />
        public LocaltunnelOptionsValidationException(string message, string property) : base(message)
        {
            this.Property = property;
        }

        public string Property { get; }
    }
}