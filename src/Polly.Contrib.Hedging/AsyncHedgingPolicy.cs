// © Microsoft Corporation. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Hedging
{

    /// <summary>
    /// A hedging policy that can be applied to delegates.
    /// </summary>
    /// The return type of delegates which may be executed through the policy.
    internal sealed class AsyncHedgingPolicy : AsyncPolicy, IsPolicy
    {
        protected override async Task<TResult> ImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }
    }
}