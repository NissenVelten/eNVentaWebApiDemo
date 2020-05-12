namespace NVShop
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NVShopException : ApplicationException
    {
        #region Constructors

        public NVShopException() {}

        public NVShopException(string message) : base(message) {}

        public NVShopException(string message, Exception innerException) : base(message, innerException) {}

        protected NVShopException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        #endregion
    }
}