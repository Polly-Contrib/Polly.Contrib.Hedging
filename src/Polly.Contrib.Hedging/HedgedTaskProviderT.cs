// © Microsoft Corporation. All rights reserved.

using System.Threading.Tasks;

namespace Polly.Contrib.Hedging
{
    /// <summary>
    /// A delegate used by the hedging policy to determine whether the next hedged task can be created.
    /// </summary>
    /// <typeparam name="TResult">Type of result returned.</typeparam>
    /// <param name="args">Arguments for the hedged task provider. See <see cref="HedgingTaskArguments"/>.</param>
    /// <param name="result">Hedged task created by the provider. <see langword="null" /> if the task was not created.</param>
    /// <returns><see langword="true" /> if a hedged task is created, <see langword="false" /> otherwise.</returns>
    public delegate bool HedgedTaskProvider<TResult>(HedgingTaskArguments args, out Task<TResult>? result);
}