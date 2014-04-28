using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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

        public TransactionAbortedException(SerializationInfo info, StreamingContext context)
        {
            timestamp = info.GetInt64("timestamp");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("timestamp", timestamp);
        }
    }
}
