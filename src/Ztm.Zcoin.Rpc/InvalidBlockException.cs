using System;
using NBitcoin;

namespace Ztm.Zcoin.Rpc
{
    public class InvalidBlockException : Exception
    {
        public InvalidBlockException(string message) : base(message)
        {
        }

        public InvalidBlockException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidBlockException(string message, uint256 block) : base(message)
        {
            Block = block;
        }

        public InvalidBlockException(string message, uint256 block, Exception innerException) : base(message, innerException)
        {
            Block = block;
        }

        public uint256 Block { get; }
    }
}