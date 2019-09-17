using DrypointException.Enum;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DrypointException
{
    [Serializable]
    public class UserFriendlyException :Exception
    {
        public string Details { get; private set; }

        public int Code { get; set; }

        public LogSeverity Severity { get; set; }

        public UserFriendlyException()
        {
            Severity = LogSeverity.Warn;
        }

        public UserFriendlyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        public UserFriendlyException(string message)
            : base(message)
        {
            Severity = LogSeverity.Warn;
        }

        public UserFriendlyException(string message, LogSeverity severity)
            : base(message)
        {
            Severity = severity;
        }

        public UserFriendlyException(int code, string message)
            : this(message)
        {
            Code = code;
        }

        public UserFriendlyException(string message, string details)
            : this(message)
        {
            Details = details;
        }

        public UserFriendlyException(int code, string message, string details)
            : this(message, details)
        {
            Code = code;
        }

        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {
            Severity = LogSeverity.Warn;
        }

        public UserFriendlyException(string message, string details, Exception innerException)
            : this(message, innerException)
        {
            Details = details;
        }
    }
}
