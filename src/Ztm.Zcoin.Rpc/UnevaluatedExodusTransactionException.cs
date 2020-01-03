using System;
using NBitcoin;

namespace Ztm.Zcoin.Rpc
{
    public class UnevaluatedExodusTransactionException : ExodusTransactionException
    {
        public UnevaluatedExodusTransactionException(string message) : base(message)
        {
        }

        public UnevaluatedExodusTransactionException(string message, uint256 transaction) : base(message, transaction)
        {
        }

        public UnevaluatedExodusTransactionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnevaluatedExodusTransactionException(string message, uint256 transaction, Exception innerException)
            : base(message, transaction, innerException)
        {
        }
    }
}