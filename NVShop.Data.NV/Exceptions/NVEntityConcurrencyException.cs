namespace NVShop.Data.NV
{
    using System;
    using System.Runtime.Serialization;

    public class NVEntityConcurrencyException : NVException
    {
        public NVEntityConcurrencyException() {}
        public NVEntityConcurrencyException(string message) : base(message) {}
        public NVEntityConcurrencyException(string message, Exception innerException) : base(message, innerException) {}
        public NVEntityConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}