namespace NVShop
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    public class Guard
    {
        #region Static Fields and Constants

        private const string AGAINST_MESSAGE = "Assertion evaluation failed with 'false'.";
        private const string IMPLEMENTS_MESSAGE = "Type '{0}' must implement type '{1}'.";
        private const string INHERITS_FROM_MESSAGE = "Type '{0}' must inherit from type '{1}'.";
        private const string IS_TYPE_OF_MESSAGE = "Type '{0}' must be of type '{1}'.";
        private const string IS_EQUAL_MESSAGE = "Compared objects must be equal.";
        private const string IS_POSITIVE_MESSAGE = "Argument '{0}' must be a positive value. Value: '{1}'.";
        private const string IS_TRUE_MESSAGE = "True expected for '{0}' but the condition was False.";
        private const string NOT_NEGATIVE_MESSAGE = "Argument '{0}' cannot be a negative value. Value: '{1}'.";

        #endregion

        [DebuggerStepThrough]
        public static void ArgumentNotNull<T>(Expression<Func<T>> arg)
        {
            if (IsNullExpression(arg))
            {
                throw new ArgumentNullException(GetParamName(arg));
            }
        }

        [DebuggerStepThrough]
        public static void ArgumentNotNegative<T>(Expression<Func<T>> arg, string message = NOT_NEGATIVE_MESSAGE) where T : IComparable<T>
        {
            if (arg.Compile().Invoke().CompareTo(default(T)) < 0)
            {
                throw Error.ArgumentOutOfRange(GetParamName(arg), message.FormatInvariant(GetParamName(arg), arg));
            }
        }
        
        [DebuggerStepThrough]
        private static string GetParamName<T>(Expression<Func<T>> expression)
        {
            var name = string.Empty;

            if (expression.Body is MemberExpression body)
            {
                name = body.Member.Name;
            }

            return name;
        }

        [DebuggerStepThrough]
        private static bool IsNullExpression(Expression exp)
        {

            // If types are different  for example int and int? there will be an extra conversion expression, we need to unwrap this
            if (exp is UnaryExpression uExp)
                exp = uExp.Operand;

            // If we are dealing with a captured variable, then teh constant will be the capture object and the value is stored as a member on this object
            if (exp is MemberExpression mExp && mExp.Expression is ConstantExpression cExp)
            {
                object value = mExp.Member is PropertyInfo pInfo ? pInfo.GetValue(cExp.Value) :
                    mExp.Member is FieldInfo fInfo ? fInfo.GetValue(cExp.Value) :
                    throw new NotSupportedException();

                return value == null;
            }
            // If we use a simple constant, this is what will be called
            if (exp is ConstantExpression constantExpression)
            {
                return constantExpression.Value == null;
            }

            return false;
        }
    }
}