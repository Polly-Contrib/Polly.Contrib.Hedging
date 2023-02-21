using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Hedging.Internals;

namespace Polly.Contrib.Hedging
{
    /// <summary>
    /// A reactive hedging policy that can be applied to delegates.
    /// </summary>
    public class AsyncHedgingPolicy<TResult> : AsyncPolicy<TResult>
    {
        private readonly HedgingEngineOptions<TResult> _hedgingEngineOptions;
        private readonly HedgedTaskProvider<TResult> _hedgedTaskProvider;

        internal AsyncHedgingPolicy(
            PolicyBuilder<TResult> policyBuilder,
            HedgedTaskProvider<TResult> hedgedTaskProvider,
            int maxHedgedTasks,
            Func<HedgingTaskArguments, TimeSpan> hedgingDelayGenerator,
            Func<DelegateResult<TResult>, Context, int, CancellationToken, Task> onHedgingAsync)
            : base(policyBuilder)
        {
            _hedgedTaskProvider = hedgedTaskProvider;
            _hedgingEngineOptions = new HedgingEngineOptions<TResult>(
                maxHedgedTasks,
                hedgingDelayGenerator,
                ExceptionPredicates,
                ResultPredicates,
                onHedgingAsync);
        }

        /// <inheritdoc/>
        protected override Task<TResult> ImplementationAsync(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return HedgingEngine<TResult>.ExecuteAsync(
                action,
                context,
                _hedgedTaskProvider,
                _hedgingEngineOptions,
                continueOnCapturedContext,
                cancellationToken);
        }
    }
}
