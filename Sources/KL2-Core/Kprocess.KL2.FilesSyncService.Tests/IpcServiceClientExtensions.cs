using JKang.IpcServiceFramework;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.FilesSyncService.Tests
{
    public static class IpcServiceClientExtensions
    {
        public static async Task InvokeWithDelayAsync<TInterface>(this IpcServiceClient<TInterface> client,
            Expression<Action<TInterface>> exp,
            TimeSpan delay,
            CancellationToken cancellationToken = default(CancellationToken))
            where TInterface : class
        {
            var task = client.InvokeAsync(exp, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(delay, cancellationToken)) != task)
                throw new TimeoutException();
        }

        public static async Task<TResult> InvokeWithDelayAsync<TInterface, TResult>(this IpcServiceClient<TInterface> client,
            Expression<Func<TInterface, TResult>> exp,
            TimeSpan delay,
            CancellationToken cancellationToken = default(CancellationToken))
            where TInterface : class
        {
            var task = client.InvokeAsync(exp, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(delay, cancellationToken)) == task)
                return task.Result;
            throw new TimeoutException();
        }

        public static async Task InvokeWithDelayAsync<TInterface>(this IpcServiceClient<TInterface> client,
            Expression<Func<TInterface, Task>> exp,
            TimeSpan delay,
            CancellationToken cancellationToken = default(CancellationToken))
            where TInterface : class
        {
            var task = client.InvokeAsync(exp, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(delay, cancellationToken)) != task)
                throw new TimeoutException();
        }

        public static async Task<TResult> InvokeWithDelayAsync<TInterface, TResult>(this IpcServiceClient<TInterface> client,
            Expression<Func<TInterface, Task<TResult>>> exp,
            TimeSpan delay,
            CancellationToken cancellationToken = default(CancellationToken))
            where TInterface : class
        {
            var task = client.InvokeAsync(exp, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(delay, cancellationToken)) == task)
                return task.Result;
            throw new TimeoutException();
        }
    }
}
