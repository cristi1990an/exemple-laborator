using System;
using System.Runtime.Serialization;

namespace Laborator_6_PSSC.Domain.Models
{
    [Serializable]
    internal class InvalidAddressException : Exception
    {
        public InvalidAddressException()
        {
        }

        public InvalidAddressException(string? message) : base(message)
        {
        }

        public InvalidAddressException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}