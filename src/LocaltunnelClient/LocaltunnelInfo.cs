// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelInfo.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using LocaltunnelClient.Internal;

namespace LocaltunnelClient
{
    public sealed class LocaltunnelInfo
    {
        public int RemotePort { get; }
        public string Id { get; }
        public string HostName { get; }

        /// <inheritdoc />
        internal LocaltunnelInfo(TunnelServerResponse serverResponse)
        {
            this.RemotePort = serverResponse.RemotePort;
            this.Id = serverResponse.Id;
            this.HostName = serverResponse.Id;
        }
    }
}