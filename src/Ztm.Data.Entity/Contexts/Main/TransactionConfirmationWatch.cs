using System;
using NBitcoin;

namespace Ztm.Data.Entity.Contexts.Main
{
    public sealed class TransactionConfirmationWatch
    {
        public Guid Id { get; set; }
        public Guid CallbackId { get; set; }
        public uint256 Transaction { get; set; }
        public int Confirmation { get; set; }
        public DateTime Due { get; set; }
        public string SuccessData { get; set; }
        public string TimeoutData { get; set; }

        public WebApiCallback Callback { get; set; }
    }
}