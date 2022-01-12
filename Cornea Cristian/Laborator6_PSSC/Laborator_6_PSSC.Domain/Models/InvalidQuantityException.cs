using System;
using System.Runtime.Serialization;

namespace Laborator_6_PSSC.Domain.Models
{
    [Serializable]
    internal class InvalidQuantityException : Exception
    {
        public InvalidQuantityException()
        {
        }

        public InvalidQuantityException(string? message) : base(message)
        {
        }

        public InvalidQuantityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidQuantityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}