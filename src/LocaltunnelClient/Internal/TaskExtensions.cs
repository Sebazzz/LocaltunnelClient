// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : TaskExtensions.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;
using System.Threading;
using System.Threading.Tasks;

namespace LocaltunnelClient.Internal
{
    internal static class TaskExtensions
    {
        public static async Task WithCancellation(
            this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>) s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            }

            await task;
        }

        public static async Task<T> WithCancellation<T>(
            this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>) s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            }

            return await task;
        }
    }
}