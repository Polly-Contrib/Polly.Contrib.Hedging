using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Hedging.Internals;

namespace Polly.Contrib.Hedging
{
    /// <summary>
    /// Fluent API for defining a Hedging <see cref="AsyncPolicy"/>.
    /// </summary>
    public static class AsyncHedgingSyntax
    {
        /// <summary>
        /// Builds an <see cref="Hedging.AsyncHedgingPolicy{TResult}" /> which provides the fastest result
        /// returned from a set of tasks (i.e. hedged execution) if the main execution fails or is too slow.
        /// If this throws a handled exception or raises a handled result,
        /// if asynchronously calls <paramref name="onHedgingAsync" />
        /// with details of the handled exception or result and the execution context;
        /// Then will continue to wait and check for the first allowed of task provided by
        /// the <paramref name="hedgedTaskProvider" /> and returns its result;
        /// If none of the tasks returned by <paramref name="hedgedTaskProvider" /> returns an allowed result,
        /// the last handled exception or result will be returned.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="hedgedTaskProvider">The hedged action provider.</param>
        /// <param name="maxHedgedTasks">The maximum hedged tasks.</param>
        /// <param name="hedgingDelayGenerator">The delegate that provides the hedging delay for each hedged task.</param>
        /// <param name="onHedgingAsync">The action to call asynchronously after invoking one hedged task.</param>
        /// <returns>
        /// The policy instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Arguments cannot be null.</exception>
        public static AsyncHedgingPolicy<TResult> AsyncHedgingPolicy<TResult>(
            this PolicyBuilder<TResult> policyBuilder,
            HedgedTaskProvider<TResult> hedgedTaskProvider,
            int maxHedgedTasks,
            Func<HedgingTaskArguments, TimeSpan> hedgingDelayGenerator,
            Func<DelegateResult<TResult>, Context, int, CancellationToken, Task> onHedgingAsync)
        {
            return new AsyncHedgingPolicy<TResult>(
                     policyBuilder,
                     hedgedTaskProvider,
                     maxHedgedTasks,
                     hedgingDelayGenerator,
                     onHedgingAsync);
        }

        /// <summary>
        /// Builds an <see cref="Hedging.AsyncHedgingPolicy" /> which completes the fastest action completed
        /// from a set of tasks (i.e. hedged execution) if the main execution fails or is too slow.
        /// If this throws a handled exception it asynchronously calls <paramref name="onHedgingAsync" />
        /// with details of the handled exception or result and the execution context;
        /// Then will continue to wait and check for the first successfully completed task provided by
        /// the <paramref name="hedgedTaskProvider" />.
        /// If none of the tasks returned by <paramref name="hedgedTaskProvider" /> comples without throwin,
        /// the last handled exception or result will be returned.
        /// </summary>
        /// <param name="policyBuilder">The policy builder.</param>
        /// <param name="hedgedTaskProvider">The hedged action provider.</param>
        /// <param name="maxHedgedTasks">The maximum hedged tasks.</param>
        /// <param name="hedgingDelayGenerator">The delegate that provides the hedging delay for each hedged task.</param>
        /// <param name="onHedgingAsync">The action to call asynchronously after invoking one hedged task.</param>
        /// <returns>
        /// The policy instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Arguments cannot be null.</exception>
        public static AsyncHedgingPolicy AsyncHedgingPolicy(
            this PolicyBuilder policyBuilder,
            HedgedTaskProvider hedgedTaskProvider,
            int maxHedgedTasks,
            Func<HedgingTaskArguments, TimeSpan> hedgingDelayGenerator,
            Func<Exception, Context, int, CancellationToken, Task> onHedgingAsync)
        {
            return new AsyncHedgingPolicy(
                     policyBuilder,
                     WrapProvider(hedgedTaskProvider),
                     maxHedgedTasks,
                     hedgingDelayGenerator,
                     (ex, ctx, task, token) => onHedgingAsync(ex.Exception, ctx, task, token));
        }

        internal static HedgedTaskProvider<EmptyStruct> WrapProvider(HedgedTaskProvider provider)
        {
            return WrappedProvider;

            bool WrappedProvider(HedgingTaskArguments args, out Task<EmptyStruct>? result)
            {
                if (provider(args, out var hedgingTask) && hedgingTask is not null)
                {
                    result = hedgingTask.ContinueWith(task => EmptyStruct.Instance, TaskScheduler.Default);
                    return true;
                }

                result = null;
                return false;
            }
        }
    }

}
