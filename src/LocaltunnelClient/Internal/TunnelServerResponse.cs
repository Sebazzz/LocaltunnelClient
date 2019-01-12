// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TunnelServerResponse.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System.Runtime.Serialization;

namespace LocaltunnelClient.Internal
{
    internal sealed class TunnelServerResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "port")]
        public int RemotePort { get; set; }

        [DataMember(Name = "max_conn_count")]
        public int MaxAllowedConnections { get; set; }
    }
}