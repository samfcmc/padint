using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM.Exceptions
{
    class TransactionAbortedException : Exception
    {
        // Transaction id
        public long timestamp
        {
            set { this.timestamp = value; }
            get { return this.timestamp;  }
        }

        public TransactionAbortedException(long timestamp, string message): base(message)
        {
            this.timestamp = timestamp;
        }
    }
}
