// © Microsoft Corporation. All rights reserved.

using System;
using System.Threading;

namespace Polly.Contrib.Hedging
{
    /// <summary>
    /// A wrapper that holds current request's <see cref="global::Polly.Context"/>
    /// and the current hedging attempt number.
    /// </summary>
    public readonly partial struct HedgingTaskArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HedgingTaskArguments"/> struct.
        /// </summary>
        /// <param name="context">Current request's context.</param>
        /// <param name="attemptNumber">Count of already executed hedging attempts.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public HedgingTaskArguments(Context context, int attemptNumber, CancellationToken cancellationToken)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            AttemptNumber = attemptNumber;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Gets the hedging attempt number.
        /// </summary>
        public int AttemptNumber { get; }

        /// <summary>
        /// Gets the Polly <see cref="global::Polly.Context" /> associated with the policy execution.
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// Gets the cancellation token associated with the policy execution.
        /// </summary>
        public CancellationToken CancellationToken { get; }
    }
}