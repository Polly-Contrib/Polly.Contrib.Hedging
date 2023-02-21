// © Microsoft Corporation. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Hedging.Internals
{
    internal sealed class HedgingEngineOptions<TResult>
    {
        public int MaxHedgedTasks { get; }

        public Func<HedgingTaskArguments, TimeSpan> HedgingDelayGenerator { get; }

        public ExceptionPredicates ShouldHandleExceptionPredicates { get; }

        public ResultPredicates<TResult> ShouldHandleResultPredicates { get; }

        public Func<DelegateResult<TResult>, Context, int, CancellationToken, Task> OnHedgingAsync { get; }

        public HedgingEngineOptions(
            int maxHedgedTasks,
            Func<HedgingTaskArguments, TimeSpan> hedgingDelayGenerator,
            ExceptionPredicates shouldHandleExceptionPredicates,
            ResultPredicates<TResult> shouldHandleResultPredicates,
            Func<DelegateResult<TResult>, Context, int, CancellationToken, Task> onHedgingAsync)
        {
            MaxHedgedTasks = maxHedgedTasks;
            ShouldHandleExceptionPredicates = shouldHandleExceptionPredicates;
            ShouldHandleResultPredicates = shouldHandleResultPredicates;
            OnHedgingAsync = onHedgingAsync;
            HedgingDelayGenerator = hedgingDelayGenerator;
        }
    }

}
