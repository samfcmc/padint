using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PADI_DSTM.Exceptions
{
    [Serializable]
    public class ReadWriteOutsideTransactionException : Exception
    {
        // Transaction id
        private long timestamp;

        public ReadWriteOutsideTransactionException(long timestamp, string message)
            : base(message)
        {
            this.timestamp = timestamp;
        }

        public ReadWriteOutsideTransactionException(SerializationInfo info, StreamingContext context)
        {
            timestamp = info.GetInt32("timestamp");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("timestamp", timestamp);
        }
    }
}
