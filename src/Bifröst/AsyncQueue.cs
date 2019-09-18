using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bifröst
{
    public class AsyncQueue<T> : IAsyncEnumerable<T>, IDisposable
    {
        private readonly SemaphoreSlim enumerationSemaphore = new SemaphoreSlim(1);
        private readonly BufferBlock<T> bufferBlock = new BufferBlock<T>();

        private bool isDisposing = false;

        public Task EnqueueAsync(T item) => this.bufferBlock.SendAsync(item);

        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default)
        {
            // We lock this so we only ever enumerate once at a time.
            // That way we ensure all items are returned in a continuous
            // fashion with no 'holes' in the data when two foreach compete.
            await this.enumerationSemaphore.WaitAsync()
                .ConfigureAwait(false);

            try
            {
                // Return new elements until cancellationToken is triggered.
                while (true)
                {
                    // Make sure to throw on cancellation so the Task will transfer into a canceled state
                    token.ThrowIfCancellationRequested();
                    yield return await this.bufferBlock.ReceiveAsync(token)
                        .ConfigureAwait(false);
                }
            }
            finally
            {
                this.enumerationSemaphore.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposing)
            {
                if (disposing)
                {
                    this.enumerationSemaphore.Dispose();
                }

                this.isDisposing = true;
            }
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
