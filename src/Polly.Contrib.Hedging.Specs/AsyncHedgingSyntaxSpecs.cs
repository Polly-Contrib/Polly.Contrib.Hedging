// © Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Polly.Contrib.Hedging.Specs
{
    public sealed class AsyncHedgingSyntaxSpecs : IDisposable
    {
        private readonly Context _context;
        private readonly CancellationTokenSource _cts;
        private string _dummyInMemoryTestCache;

        public AsyncHedgingSyntaxSpecs()
        {
            _context = new Context();
            _cts = new CancellationTokenSource();
            _dummyInMemoryTestCache = string.Empty;
        }

        public void Dispose()
        {
            _cts.Dispose();
        }

        [Fact]
        public async Task ReactiveAsyncHedgingPolicy_CreateAndExecuteTypedPolicy_ShouldReturnFastestResult()
        {
            HedgedTaskProvider<string> hedgedTaskProvider = ProvideReactiveHedgedTask;

            var hedgingPolicy = Policy
                .Handle<Exception>()
                .OrResult<string>(_ => false)
                .AsyncHedgingPolicy(
                    hedgedTaskProvider,
                    4,
                    Utilities.DefaultHedgingDelayGenerator,
                    Utilities.Reactive<string>.EmptyOnHedgingTask);

            Assert.NotNull(hedgingPolicy);

            var result = await hedgingPolicy.ExecuteAsync(
                () =>
                Utilities.StringTasks.FastTask(_cts.Token));

            Assert.Equal(Utilities.StringTasks.FastTaskExpectedResult, result);
        }

        [Fact]
        public async Task ReactiveAsyncHedgingPolicy_CreateAndExecuteTypedPolicy_ShouldNotReturnSlowResult()
        {
            HedgedTaskProvider<string> hedgedTaskProvider = ProvideReactiveHedgedTask;

            var hedgingPolicy = Policy
                .Handle<Exception>()
                .OrResult<string>(_ => false)
                .AsyncHedgingPolicy(
                    hedgedTaskProvider,
                    4,
                    Utilities.DefaultHedgingDelayGenerator,
                    Utilities.Reactive<string>.EmptyOnHedgingTask);

            Assert.NotNull(hedgingPolicy);

            var result = await hedgingPolicy.ExecuteAsync(
                () =>
                Utilities.StringTasks.SlowTask(_context, _cts.Token));

            Assert.NotEqual(Utilities.StringTasks.SlowTaskExpectedResult, result);
            Assert.Contains(result, new[] { "Oranges", "Pears", "Apples" });
        }


        [Fact]
        public async Task ProactiveAsyncHedgingPolicy_CreateAndExecuteTypedPolicy_ShouldCacheFastestResult()
        {
            HedgedTaskProvider hedgedTaskProvider = ProvideProactiveHedgedTask;

            var hedgingPolicy = Policy
                .Handle<Exception>()
                .AsyncHedgingPolicy(
                    hedgedTaskProvider,
                    4,
                    Utilities.DefaultHedgingDelayGenerator,
                    Utilities.Proactive.EmptyOnHedgingTask);

            Assert.NotNull(hedgingPolicy);

            await hedgingPolicy.ExecuteAsync(async () =>
               _dummyInMemoryTestCache = await Utilities.StringTasks.FastTask(_cts.Token));

            Assert.Equal(Utilities.StringTasks.FastTaskExpectedResult, _dummyInMemoryTestCache);
        }

        [Fact]
        public async Task ProactiveAsyncHedgingPolicy_CreateAndExecuteTypedPolicy_ShouldNotCacheSlowResult()
        {
            HedgedTaskProvider hedgedTaskProvider = ProvideProactiveHedgedTask;

            var hedgingPolicy = Policy
                .Handle<Exception>()
                .AsyncHedgingPolicy(
                    hedgedTaskProvider,
                    4,
                    Utilities.DefaultHedgingDelayGenerator,
                    Utilities.Proactive.EmptyOnHedgingTask);

            Assert.NotNull(hedgingPolicy);

            await hedgingPolicy.ExecuteAsync(async () =>
               _dummyInMemoryTestCache = await Utilities.StringTasks.SlowTask(_context, _cts.Token));

            Assert.NotEqual(Utilities.StringTasks.SlowTaskExpectedResult, _dummyInMemoryTestCache);
            Assert.Contains(_dummyInMemoryTestCache, new[] { "Oranges", "Pears", "Apples" });
        }

        private bool ProvideReactiveHedgedTask(HedgingTaskArguments args, out Task<string>? result)
        {
            var stringReturnTypeFunctions = Utilities.Reactive<string>.TaskFunctions;
            var maxAttempts = stringReturnTypeFunctions.Count + 1;

            if (args.AttemptNumber < maxAttempts)
            {
                var function = stringReturnTypeFunctions![args.AttemptNumber - 1];
                result = function(args.Context, args.CancellationToken)!;
                return true;
            }

            result = null!;
            return false;
        }

        private bool ProvideProactiveHedgedTask(HedgingTaskArguments args, out Task? result)
        {
            List<Func<Context, CancellationToken, Task?>> voidFunctions = Utilities.Reactive<string>.TaskFunctions
                .Select<Func<Context, CancellationToken, Task<string>?>, Func<Context, CancellationToken, Task?>>(function =>
                    (Context cx, CancellationToken ct) => (Task)ToProactiveTask(cx, ct, function))
                .ToList();

            var maxAttempts = voidFunctions.Count + 1;

            if (args.AttemptNumber < maxAttempts)
            {
                var function = voidFunctions![args.AttemptNumber - 1];
                result = function(args.Context, args.CancellationToken)!;
                return true;
            }

            result = null!;
            return false;
        }

        private async Task ToProactiveTask(Context context, CancellationToken cancellationToken, Func<Context, CancellationToken, Task<string>?> func)
        {
            var result = await func(context, cancellationToken)!;
            _dummyInMemoryTestCache = result;
        }
    }
}
