// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelDeadEvent.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Net.Sockets;

namespace LocaltunnelClient.Events
{
    public sealed class TunnelDeadEvent : TunnelOriginatedEvent
    {
        public Exception Exception { get; }

        /// <inheritdoc />
        public TunnelDeadEvent(int tunnelId, Exception exception) : base(MakeMessage(exception), tunnelId)
        {
            this.Exception = exception;
        }

        private static string MakeMessage(Exception exception)
        {
            if (exception.GetBaseException() is SocketException socketException)
            {
                return $"Tunnel dead - socket error {socketException.SocketErrorCode} - 0x{exception.HResult:x8}";
            }
                 
            return $"Tunnel dead - 0x{exception.HResult:x8}";
        }
    }
}