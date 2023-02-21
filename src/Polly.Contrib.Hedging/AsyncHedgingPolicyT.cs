using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Hedging
{
    /// <summary>
    /// A reactive hedging policy that can be applied to delegates.
    /// </summary>
    public class AsyncHedgingPolicy<TResult> : AsyncPolicy<TResult>
    {
        protected override Task<TResult> ImplementationAsync(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context, CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }
    }
}
