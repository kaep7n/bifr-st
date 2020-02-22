﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bifröst
{
    public class AsyncAutoResetEvent
    {
        private readonly LinkedList<TaskCompletionSource<bool>> waiters = new LinkedList<TaskCompletionSource<bool>>();
        private bool isSignaled;

        public AsyncAutoResetEvent(bool signaled)
            => this.isSignaled = signaled;

        public Task<bool> WaitAsync(TimeSpan timeout)
            => this.WaitAsync(timeout, CancellationToken.None);

        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs;

            lock (this.waiters)
            {
                if (this.isSignaled)
                {
                    this.isSignaled = false;
                    return true;
                }
                else if (timeout == TimeSpan.Zero)
                {
                    return this.isSignaled;
                }
                else
                {
                    tcs = new TaskCompletionSource<bool>();
                    this.waiters.AddLast(tcs);
                }
            }

            var winner = await Task.WhenAny(tcs.Task, Task.Delay(timeout, cancellationToken))
                .ConfigureAwait(false);

            if (winner == tcs.Task)
            {
                // The task was signaled.
                return true;
            }
            else
            {
                // We timed-out; remove our reference to the task.
                // This is an O(n) operation since waiters is a LinkedList<T>.
                lock (this.waiters)
                {
                    this.waiters.Remove(tcs);

                    if(winner.IsCanceled)
                    {
                        throw new TaskCanceledException();
                    }

                    return false;
                }
            }
        }

        public void Set()
        {
            lock (this.waiters)
            {
                if (this.waiters.Count > 0)
                {
                    // Signal the first task in the waiters list. This must be done on a new
                    // thread to avoid stack-dives and situations where we try to complete the
                    // same result multiple times.
                    var tcs = this.waiters.First.Value;
                    Task.Run(() => tcs.SetResult(true));
                    this.waiters.RemoveFirst();
                }
                else if (!this.isSignaled)
                {
                    // No tasks are pending
                    this.isSignaled = true;
                }
            }
        }

        public override string ToString()
            => $"Signaled: {this.isSignaled.ToString()}, Waiters: {this.waiters.Count.ToString()}";
    }
}
