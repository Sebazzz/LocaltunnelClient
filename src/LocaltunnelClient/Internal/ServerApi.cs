// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : ServerApi.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LocaltunnelClient.Internal
{
    internal static class ServerApi
    {
        /// <summary>
        /// Requests a local tunnel at the specified address
        /// </summary>
        /// <param name="server"></param>
        /// <param name="subDomain"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="LocaltunnelInitializationException"></exception>
        public static async Task<TunnelServerResponse> RequestTunnelAsync(string server, string subDomain, CancellationToken cancellationToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(server, UriKind.Absolute));

                if (string.IsNullOrEmpty(subDomain))
                {
                    uriBuilder.Query = "new";
                }
                else
                {
                    uriBuilder.Path = subDomain;
                }

                HttpResponseMessage responseMessage =
                    await httpClient.PostAsync(uriBuilder.Uri, new StringContent(string.Empty), cancellationToken);

                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    throw new LocaltunnelInitializationException($"Unable to request local tunnel at URL {uriBuilder}", ex);
                }

                string rawResponse = await responseMessage.Content.ReadAsStringAsync();

                try
                {
                    TunnelServerResponse response = JsonConvert.DeserializeObject<TunnelServerResponse>(rawResponse);

                    if (response.MaxAllowedConnections < 1 || response.RemotePort == 0)
                    {
                        throw new InvalidOperationException("Invalid server response: " + rawResponse);
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    throw new LocaltunnelInitializationException($"Unable to request local tunnel at URL {uriBuilder}", ex);
                }
            }
        }
    }
}