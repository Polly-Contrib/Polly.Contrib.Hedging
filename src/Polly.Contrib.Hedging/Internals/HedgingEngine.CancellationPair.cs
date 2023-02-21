// © Microsoft Corporation. All rights reserved.

using System;
using System.Threading;

namespace Polly.Contrib.Hedging.Internals
{
    internal static partial class HedgingEngine<TResult>
    {
        public record struct CancellationPair(CancellationTokenSource Cancellation, CancellationTokenRegistration? Registration) : IDisposable
        {
            public CancellationToken CancellationToken => Cancellation.Token;

            public static CancellationPair Create(CancellationToken token)
            {
                var currentCancellation = _cancellationSources.Get();

                if (token.CanBeCanceled)
                {
                    return new CancellationPair(currentCancellation, token.Register(o => ((CancellationTokenSource)o!).Cancel(), currentCancellation));
                }

                return new CancellationPair(currentCancellation, null);
            }

            public void Dispose()
            {
                Registration?.Dispose();
                _cancellationSources.Return(Cancellation);
            }
        }
    }
}