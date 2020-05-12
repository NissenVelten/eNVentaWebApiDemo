namespace NVShop
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    public static class Error
    {
        [DebuggerStepThrough]
        public static Exception ArgumentNullOrEmpty(string argName)
        {
            return new ArgumentException("String parameter '{0}' cannot be null or all whitespace.", argName);
        }

        [DebuggerStepThrough]
        public static Exception ArgumentOutOfRange(string argName, string message, params object[] args)
        {
            return new ArgumentOutOfRangeException(argName, string.Format(CultureInfo.CurrentCulture, message, args));
        }

        [DebuggerStepThrough]
        public static Exception Argument(string argName, string message, params object[] args)
        {
            return new ArgumentException(string.Format(CultureInfo.CurrentCulture, message, args), argName);
        }

        [DebuggerStepThrough]
        public static Exception Argument<T>(Func<T> arg, string message, params object[] args)
        {
            var argName = arg.Method.Name;
            return new ArgumentException(message.FormatCurrent(args), argName);
        }

        [DebuggerStepThrough]
        public static Exception InvalidOperation(string message, params object[] args)
        {
            return InvalidOperation(message, null, args);
        }

        [DebuggerStepThrough]
        public static Exception InvalidOperation(string message, Exception innerException, params object[] args)
        {
            return new InvalidOperationException(message.FormatCurrent(args), innerException);
        }
    }
}