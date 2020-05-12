namespace NVShop.Data.NV
{
    using System;
    using System.Runtime.Serialization;

    public class NVEntityNotFoundException : NVException
    {
        public NVEntityNotFoundException() {}
        public NVEntityNotFoundException(string message) : base(message) {}
        public NVEntityNotFoundException(string message, Exception innerException) : base(message, innerException) {}
        public NVEntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}