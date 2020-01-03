using System;
using NBitcoin;

namespace Ztm.Zcoin.Rpc
{
    public class ExodusTransactionException : Exception
    {
        public ExodusTransactionException(string message) : base(message)
        {
        }

        public ExodusTransactionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ExodusTransactionException(string message, uint256 transaction) : base(message)
        {
            Transaction = transaction;
        }

        public ExodusTransactionException(string message, uint256 transaction, Exception innerException) : base(message, innerException)
        {
            Transaction = transaction;
        }

        public uint256 Transaction { get; }
    }
}