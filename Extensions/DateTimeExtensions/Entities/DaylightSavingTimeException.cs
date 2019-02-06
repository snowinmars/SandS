using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DateTimeExtension.Entities
{
    public class DaylightSavingTimeException : Exception
    {
        public DaylightSavingTimeException()
        {
        }

        public DaylightSavingTimeException(string message) : base(message)
        {
        }

        protected DaylightSavingTimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DaylightSavingTimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
