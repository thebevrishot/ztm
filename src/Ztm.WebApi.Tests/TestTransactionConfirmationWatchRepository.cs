using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ztm.Zcoin.Watching;
using Rule = Ztm.WebApi.TransactionConfirmationWatchingRule<Ztm.WebApi.TransactionConfirmationCallbackResult>;

namespace Ztm.WebApi.Tests
{
    public class TestTransactionConfirmationWatchRepository : ITransactionConfirmationWatchRepository
    {
        readonly Dictionary<Guid, TransactionWatchWithStatus> watches;

        public TestTransactionConfirmationWatchRepository()
        {
            this.watches = new Dictionary<Guid, TransactionWatchWithStatus>();
        }

        public virtual Task AddAsync(TransactionWatch<Rule> watch, CancellationToken cancellationToken)
        {
            this.watches.Add(watch.Id, new TransactionWatchWithStatus
            {
                watch = watch,
                status = TransactionConfirmationWatchingWatchStatus.Pending
            });

            return Task.FromResult(watch);
        }

        public virtual Task<IEnumerable<TransactionWatch<Rule>>> ListAsync(TransactionConfirmationWatchingWatchStatus status, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.watches.Where(w => status.HasFlag(w.Value.status)).Select(w => w.Value.watch));
        }

        public virtual Task UpdateStatusAsync(Guid id, TransactionConfirmationWatchingWatchStatus status, CancellationToken cancellationToken)
        {
            if (this.watches.TryGetValue(id, out var watchWithStatus))
            {
                watchWithStatus.status = status;
                return Task.CompletedTask;
            }

            throw new KeyNotFoundException("Watch Id is not exist.");
        }

        class TransactionWatchWithStatus
        {
            public TransactionWatch<Rule> watch { get; set; }
            public TransactionConfirmationWatchingWatchStatus status { get; set; }
        }
    }
}