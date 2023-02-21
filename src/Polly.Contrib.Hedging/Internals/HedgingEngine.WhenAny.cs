// © Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Polly.Contrib.Hedging.Internals
{
    internal static partial class HedgingEngine<TResult>
    {
        private static Task<Task<TResult>> WhenAnyAsync(Dictionary<Task<TResult>, CancellationPair>.KeyCollection tasks)
        {
            return tasks.Count switch
            {
                1 => WhenAny1Async(tasks),
                2 => WhenAny2Async(tasks),
                _ => Task.WhenAny(tasks)
            };
        }

        static async Task<Task<TResult>> WhenAny1Async(Dictionary<Task<TResult>, CancellationPair>.KeyCollection tasks)
        {
            using var enumerator = tasks.GetEnumerator();
            _ = enumerator.MoveNext();

            try
            {
                _ = await enumerator.Current.ConfigureAwait(false);
            }
            catch (Exception)
            {
                // discard exception and propagate it in task
            }

            return enumerator.Current;
        }

        static Task<Task<TResult>> WhenAny2Async(Dictionary<Task<TResult>, CancellationPair>.KeyCollection tasks)
        {
            using var enumerator = tasks.GetEnumerator();

            _ = enumerator.MoveNext();
            var first = enumerator.Current;

            _ = enumerator.MoveNext();
            var second = enumerator.Current;

            return Task.WhenAny(first, second);
        }
    }
}