using System;
using System.Runtime.Serialization;

namespace Laborator4_PSSC.Domain
{
        [Serializable]
        internal class InvalidProductCodeException : Exception
        {
            public InvalidProductCodeException()
            {
            }

            public InvalidProductCodeException(string? message) : base(message)
            {
            }

            public InvalidProductCodeException(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected InvalidProductCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
}
