using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DrypointException
{
    public class AuthorizationException:Exception
    {

        public AuthorizationException()
        {
            
        }

        public AuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        public AuthorizationException(string message)
            : base(message)
        {

        }

        public AuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
