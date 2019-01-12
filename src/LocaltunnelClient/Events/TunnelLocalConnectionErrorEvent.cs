// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelLocalConnectionErrorEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Net.Sockets;

namespace LocaltunnelClient.Events
{
    public sealed class TunnelLocalConnectionErrorEvent : TunnelOriginatedEvent
    {
        public Exception Exception { get; }

        /// <inheritdoc />
        public TunnelLocalConnectionErrorEvent(int tunnelId, Exception exception) : base(MakeMessage(exception), tunnelId)
        {
            this.Exception = exception;
        }

        private static string MakeMessage(Exception exception)
        {
            if (exception is SocketException socketException)
            {
                return $"Tunnel not able to connect locally - {socketException.SocketErrorCode} - 0x{exception.HResult:x8}";
            }
                 
            return $"Tunnel not able to connect locally - 0x{exception.HResult:x8}";
        }
    }
}