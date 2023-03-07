using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;

namespace Polly.Contrib.Hedging.Specs
{

    public static class Utilities
    {
        public static TimeSpan DefaultHedgingDelay { get; } = TimeSpan.FromSeconds(1);

        public static Func<HedgingTaskArguments, TimeSpan> DefaultHedgingDelayGenerator { get; } = (_) => DefaultHedgingDelay;

        public static class StringTasks
        {
            public const string InstantTaskExpectedResult = "Instant";

            public const string FastTaskExpectedResult = "I am fast!";

            public const string SlowTaskExpectedResult = "I am so slow!";

            public static Task<string> InstantTask()
            {
                return Task.FromResult(InstantTaskExpectedResult);
            }

            public static async Task<string> FastTask(CancellationToken token)
            {
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(10), token);
                return FastTaskExpectedResult;
            }

            public static async Task<string> SlowTask(Context _, CancellationToken token)
            {
                await SystemClock.SleepAsync(TimeSpan.FromDays(1), token);
                return SlowTaskExpectedResult;
            }
        }

        public static class Reactive<T>
        {
            public static Func<DelegateResult<T>, Context, int, CancellationToken, Task> EmptyOnHedgingTask { get; } =
            (_, _, _, _) => Task.CompletedTask;

            public static List<Func<Context, CancellationToken, Task<T>?>> TaskFunctions { get; } =
                new()
                {
                    GetApples,
                    GetOranges,
                    GetPears
                };

            private static async Task<T> GetApples(Context context, CancellationToken token)
            {
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(10 * 1000), token);
                return (T)Convert.ChangeType("Apples", typeof(T));
            }

            private static async Task<T> GetPears(Context context, CancellationToken token)
            {
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(3 * 1000), token);
                return (T)Convert.ChangeType("Pears", typeof(T));
            }

            private static async Task<T> GetOranges(Context context, CancellationToken token)
            {
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(2 * 1000), token);
                return (T)Convert.ChangeType("Oranges", typeof(T));
            }
        }

        public static class Proactive
        {
            public static Func<Exception, Context, int, CancellationToken, Task> EmptyOnHedgingTask { get; } =
                (_, _, _, _) => Task.CompletedTask;
        }
    }
}