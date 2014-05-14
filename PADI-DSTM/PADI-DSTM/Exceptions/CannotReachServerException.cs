using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PADI_DSTM.Exceptions
{
    [Serializable]
    public class CannotReachServerException : Exception
    {
        public CannotReachServerException(string message)
            : base(message)
        {
        }

        public CannotReachServerException(SerializationInfo info, StreamingContext context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
