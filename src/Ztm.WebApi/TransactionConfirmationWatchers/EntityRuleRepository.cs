using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using Newtonsoft.Json;
using Ztm.Data.Entity.Contexts;
using TransactionConfirmationWatchingRuleModel = Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatchingRule;
using Ztm.WebApi.Callbacks;

namespace Ztm.WebApi.TransactionConfirmationWatchers
{
    public class EntityRuleRepository : IRuleRepository
    {
        readonly IMainDatabaseFactory db;

        public EntityRuleRepository(IMainDatabaseFactory db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            this.db = db;
        }

        public async Task<Rule> AddAsync(
            uint256 transaction, int confirmation, TimeSpan waitingTime, CallbackResult successResponse, CallbackResult timeoutResponse, Callback callback, CancellationToken cancellationToken)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (successResponse == null)
            {
                throw new ArgumentNullException(nameof(successResponse));
            }

            if (timeoutResponse == null)
            {
                throw new ArgumentNullException(nameof(timeoutResponse));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            using (var db = this.db.CreateDbContext())
            {
                var watch = await db.TransactionConfirmationWatchingRules
                    .AddAsync(
                    new TransactionConfirmationWatchingRuleModel
                    {
                        Id = Guid.NewGuid(),
                        CallbackId = callback.Id,
                        Transaction = transaction,
                        Status = (int)RuleStatus.Pending,
                        Confirmation = confirmation,
                        WaitingTime = waitingTime,
                        RemainingWaitingTime = waitingTime,
                        SuccessData = JsonConvert.SerializeObject(successResponse),
                        TimeoutData = JsonConvert.SerializeObject(timeoutResponse),
                        CurrentWatchId = null,
                    }, cancellationToken);

                await db.SaveChangesAsync(cancellationToken);

                return ToDomain(watch.Entity, callback);
            }
        }

        public async Task<Rule> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            using (var db = this.db.CreateDbContext())
            {
                var watch = await db.TransactionConfirmationWatchingRules
                    .Include(e => e.Callback)
                    .Include(e => e.CurrentWatch)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                return watch == null ? null : ToDomain(watch);
            }
        }

        public async Task<IEnumerable<Rule>> ListActiveAsync(CancellationToken cancellationToken)
        {
            using (var db = this.db.CreateDbContext())
            {
                var watches = await db.TransactionConfirmationWatchingRules
                    .Include(e => e.Callback)
                    .Include(e => e.CurrentWatch)
                    .Where(e => e.Status == (int)WatchStatus.Pending)
                    .ToListAsync(cancellationToken);

                return watches.Select(e => ToDomain(e));
            }
        }

        public async Task SubtractRemainingWaitingTimeAsync(Guid id, TimeSpan consumedTime, CancellationToken cancellationToken)
        {
            using (var db = this.db.CreateDbContext())
            {
                var watch = await db.TransactionConfirmationWatchingRules
                    .Include(e => e.Callback)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (watch == null)
                {
                    throw new KeyNotFoundException("Watch id not found.");
                }

                if (consumedTime < TimeSpan.Zero)
                {
                    throw new ArgumentException("Consumed time could not be negative.");
                }

                watch.RemainingWaitingTime -= consumedTime;
                watch.RemainingWaitingTime = watch.RemainingWaitingTime < TimeSpan.Zero
                    ? TimeSpan.Zero
                    : watch.RemainingWaitingTime;

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateStatusAsync(Guid id, RuleStatus status, CancellationToken cancellationToken)
        {
            using (var db = this.db.CreateDbContext())
            {
                var watch = await db.TransactionConfirmationWatchingRules
                    .Include(e => e.Callback)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (watch == null)
                {
                    throw new KeyNotFoundException("Watch id not found");
                }

                watch.Status = (int)status;

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TimeSpan> GetRemainingWaitingTimeAsync(Guid id, CancellationToken cancellationToken)
        {
            using (var db = this.db.CreateDbContext())
            {
                var watch = await db.TransactionConfirmationWatchingRules
                    .Include(e => e.Callback)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (watch == null)
                {
                    throw new KeyNotFoundException("Watch id not found.");
                }

                return watch.RemainingWaitingTime;
            }
        }

        public static Rule ToDomain(
            Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatchingRule watch,
            Callback callback = null)
        {
            return new Rule(
                watch.Id,
                watch.Transaction,
                (RuleStatus)watch.Status,
                watch.Confirmation,
                watch.WaitingTime,
                JsonConvert.DeserializeObject(watch.SuccessData),
                JsonConvert.DeserializeObject(watch.TimeoutData),
                callback != null
                    ? callback
                    : (watch.Callback == null ? null : EntityCallbackRepository.ToDomain(watch.Callback)),
                watch.CurrentWatchId
            );
        }
    }
}