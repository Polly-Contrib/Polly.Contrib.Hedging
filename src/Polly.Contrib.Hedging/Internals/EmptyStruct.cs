// © Microsoft Corporation. All rights reserved.

namespace Polly.Contrib.Hedging.Internals
{
    /// <summary>
    /// A null struct for policies and actions which do not return a TResult.
    /// </summary>
    internal readonly struct EmptyStruct
    {
        /// <summary>
        /// Initializes a new instance of the EmptyStruct for policies which do not return a result." /> structure.
        /// </summary>
#pragma warning disable CS0649 // Filed is never assigned to - that is the point.
        internal static readonly EmptyStruct Instance;
#pragma warning restore CS0649
    }
}
