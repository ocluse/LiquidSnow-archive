using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Extensions for the task Namespace
    /// </summary>
    public static class TaskExtensions
    {
        ///<inheritdoc cref="TimeoutAfter(Task, TimeSpan)"/>
        public static Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, double milliseconds)
        {
            return task.TimeoutAfter(TimeSpan.FromMilliseconds(milliseconds));
        }

        ///<inheritdoc cref="TimeoutAfter(Task, TimeSpan)"/>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {

            using CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task;  // Very important in order to propagate exceptions
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }

        ///<inheritdoc cref="TimeoutAfter(Task, TimeSpan)"/>
        public static Task TimeoutAfter(this Task task, double milliseconds)
        {
            return task.TimeoutAfter(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Timeout a task after the provided duration.
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {

            using CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                await task;  // Very important in order to propagate exceptions
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }
    }
}
