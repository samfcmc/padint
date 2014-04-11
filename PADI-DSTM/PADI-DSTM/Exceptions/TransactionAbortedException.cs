using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM.Exceptions
{
    [Serializable]
    public class TransactionAbortedException : Exception
    {
        // Transaction id
        private long timestamp;

        public TransactionAbortedException(long timestamp, string message): base(message)
        {
            this.timestamp = timestamp;
        }
    }
}
