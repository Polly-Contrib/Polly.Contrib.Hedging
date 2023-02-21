// © Microsoft Corporation. All rights reserved.

using Microsoft.Extensions.ObjectPool;

namespace Polly.Contrib.Hedging.Internals
{
    internal static class PoolFactory
    {
        private static ObjectPoolProvider _poolProvider = new DefaultObjectPoolProvider();

        public static ObjectPool<T> Create<T>() where T : class, new()
        {
            return _poolProvider.Create<T>();
        }
    }
}
