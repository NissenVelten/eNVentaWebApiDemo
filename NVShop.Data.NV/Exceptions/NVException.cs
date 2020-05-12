namespace NVShop.Data.NV
{
    using System;
    using System.Runtime.Serialization;

    public class NVException : ApplicationException, ISerializable
    {
        public NVException() {}
        public NVException(string message) : base(message) {}
        public NVException(string message, Exception innerException) : base(message, innerException) {}
        public NVException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}