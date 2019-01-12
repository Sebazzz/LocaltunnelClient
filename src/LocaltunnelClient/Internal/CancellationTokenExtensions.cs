// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : CancellationTokenExtensions.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System.Threading;

namespace LocaltunnelClient.Internal
{
    internal static class CancellationTokenExtensions
    {
        public static void Chain(this CancellationTokenSource cancellationTokenSource,
                                 ref CancellationToken cancellationToken)
        {
            cancellationToken.Register(cancellationTokenSource.Cancel);
            cancellationToken = cancellationTokenSource.Token;
        }
    }
}