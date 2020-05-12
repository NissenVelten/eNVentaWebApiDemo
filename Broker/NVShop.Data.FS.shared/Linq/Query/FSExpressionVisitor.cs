namespace NVShop.Data.FS.Linq.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Text;
	using FrameworkSystems.FrameworkBase.Metadatatype;
	using FrameworkSystems.FrameworkDataProvider;
    using Remotion.Linq.Clauses.Expressions;
    using Remotion.Linq.Parsing;

    public class FSExpressionVisitor : RelinqExpressionVisitor
    {
        public static string GetFSExpression(Expression node)
        {
            var visitor = new FSExpressionVisitor();
			visitor.Visit(node);
            return visitor.GetFSExpression();
        }

        List<string> supportedMethods = new List<string>
        {
            "Contains",
            "Like"
        };

        private readonly StringBuilder _fsExpression = new StringBuilder();

        public string GetFSExpression()
        {
            return _fsExpression.ToString();
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            return expression;
        }


        /// <summary>Wechselt zu den untergeordneten Elementen der <see cref="T:System.Linq.Expressions.MemberInitExpression" />.</summary>
        /// <returns>Der geänderte Ausdruck, wenn dieser oder einer seiner Teilausdrücke geändert wurde. Andernfalls wird der ursprüngliche Ausdruck zurückgegeben.</returns>
        /// <param name="node">Der Ausdruck, zu dem gewechselt werden soll.</param>
        protected override Expression VisitMemberInit(MemberInitExpression expression)
        {
            return base.VisitMemberInit(expression);
        }


        protected override Expression VisitBinary(BinaryExpression expression)
        {
            //_fsExpression.Append("(");

            var leftExpr = Visit(expression.Left);
			switch (expression.NodeType)
			{
				case ExpressionType.Equal:
					if (expression.Right == null)
					{
						_fsExpression.Append(" IS NULL ");
					}
					else
					{
						_fsExpression.Append(" = ");
					}

					break;
				case ExpressionType.NotEqual:
					if (expression.Right == null)
					{
						_fsExpression.Append(" NOT IS NULL ");
					}
					else
					{
						_fsExpression.Append(" != ");
					}
					break;
				case ExpressionType.AndAlso:
				case ExpressionType.And:
					_fsExpression.Append(" AND ");
					break;

				case ExpressionType.OrElse:
				case ExpressionType.Or:
					_fsExpression.Append(" OR ");
					break;

				//            case ExpressionType.Add:
				//                _fsExpression.Append(" + ");
				//                break;

				//            case ExpressionType.Subtract:
				//                _fsExpression.Append(" - ");
				//                break;

				//            case ExpressionType.Multiply:
				//                _fsExpression.Append(" * ");
				//                break;

				//            case ExpressionType.Divide:
				//                _fsExpression.Append(" / ");
				//                break;

				default:
					base.VisitBinary(expression);
					break;
			}


			var rightExpr = Visit(expression.Right);

			//_fsExpression.Append(")");

			return expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            Visit(expression.Expression);
            _fsExpression.AppendFormat("[{0}]", expression.Member.Name);

            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
			if (expression?.Value != null)
			{
				_fsExpression.AppendFormat($"{DB.SqlStringObject(expression.Value)}");
			}

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            // In production code, handle this via method lookup tables.
            //var obj = Visit(expression.Object);

            //var args = VisitExpressionList(expression.Arguments);
            //if (obj != expression.Object || args != expression.Arguments)
            //{
            //    return Expression.Call(obj, expression.Method, args);
            //}

            //return expression;
            var supportedMethod = typeof(FSstring).GetMethod("StartsWith");
            if (expression.Method.Equals(supportedMethod))
            {
                _fsExpression.Append("(");
                Visit(expression.Object);
                _fsExpression.Append(" like '");
                Visit(expression.Arguments[0]);
                _fsExpression.Append("+'%')");
                return expression;
            }
            return base.VisitMethodCall(expression); // throws
        }

        /// <summary>
        /// Expression list visit method
        /// </summary>
        /// <param name="original">The expression list to visit</param>
        /// <returns>The visited expression list</returns>
        public virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            if (list != null)
            {
                return new ReadOnlyCollection<Expression>(list);
            }

            return original;
        }

        //// Called when a LINQ expression type is not handled above.
        //protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        //{
        //    var itemText = FormatUnhandledItem(unhandledItem);
        //    var message = $"The expression '{itemText}' (type: {typeof(T)}) is not supported by this LINQ provider.";
        //    return new NotSupportedException(message);
        //}

        /// <summary>Wechselt zu den untergeordneten Elementen der <see cref="T:System.Linq.Expressions.UnaryExpression" />.</summary>
        /// <returns>Der geänderte Ausdruck, wenn dieser oder einer seiner Teilausdrücke geändert wurde. Andernfalls wird der ursprüngliche Ausdruck zurückgegeben.</returns>
        /// <param name="node">Der Ausdruck, zu dem gewechselt werden soll.</param>
        protected override Expression VisitUnary(UnaryExpression expression)
        {
            return base.VisitUnary(expression);
        }

        private static string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression?.ToString() ?? unhandledItem.ToString();
        }
    }
}