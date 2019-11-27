using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Ztm.Hosting
{
    public abstract class BackgroundService : IDisposable, IHostedService
    {
        readonly IBackgroundServiceExceptionHandler exceptionHandler;
        readonly CancellationTokenSource cancellation;
        Task background;
        bool disposed;

        protected BackgroundService(IBackgroundServiceExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            this.exceptionHandler = exceptionHandler;
            this.cancellation = new CancellationTokenSource();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.background != null)
            {
                throw new InvalidOperationException("The service is already started.");
            }

            this.background = ExecuteAsync(this.cancellation.Token).ContinueWith(FinalizeBackgroundAsync);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.background == null)
            {
                throw new InvalidOperationException("The service was not started.");
            }

            this.cancellation.Cancel();

            try
            {
                var completed = await Task.WhenAny(
                    this.background,
                    Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken)
                );

                await completed;
            }
            finally
            {
                this.background = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.background != null)
                {
                    StopAsync(CancellationToken.None).Wait();
                }

                this.cancellation.Dispose();
            }

            this.disposed = true;
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        Task FinalizeBackgroundAsync(Task background)
        {
            if (background.IsFaulted)
            {
                return this.exceptionHandler.RunAsync(
                    GetType(),
                    background.Exception.InnerException,
                    CancellationToken.None
                );
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
