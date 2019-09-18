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
            await this.enumerationSemaphore.WaitAsync()
                .ConfigureAwait(false);

            try
            {
                while (true)
                {
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
