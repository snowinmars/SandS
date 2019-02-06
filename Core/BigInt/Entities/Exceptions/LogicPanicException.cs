using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BigInt.Entities
{
    public class LogicPanicException : Exception
    {
        public LogicPanicException()
        {
        }

        public LogicPanicException(string message) : base(message)
        {
        }

        public LogicPanicException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogicPanicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
