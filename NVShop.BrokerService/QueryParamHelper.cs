using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NVShop.Data.NV;

namespace NVShop.BrokerService
{
	/// <summary>
	/// 
	/// </summary>
	public static class QueryParamHelper
	{
		/// <summary>
		/// Applies the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="values">The parameter.</param>
		/// <exception cref="NVShopException">
		/// </exception>
		public static void Apply(object obj, string values)
		{
			var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var token in QueryParamTokenizer.Tokenize(values))
            {
                var method = methods.FirstOrDefault(x => x.Name == token.MethodName);
                if (method == null)
                {
                    throw new NVShopException($"Invalid Method {token.MethodName}({string.Join(",", token.Values)})");
                }

                var parameters = method.GetParameters();
                if (parameters.Length != token.Values.Count && !parameters.Any(p => p.HasAttribute<ParamArrayAttribute>(true)))
                {
                    throw new NVShopException($"Invalid count of Arguments on Method {token.MethodName}({string.Join(",",token.Values)})");
                }

                var arguments = new List<object>();
                for (var index = 0; index < token.Values.Count; index++)
                {
                    var param = parameters[index];
                    if (param.CustomAttributes.Any(x => x.AttributeType == typeof(ParamArrayAttribute)))
                    {
                        var @params = new List<object>();
                        while (index < token.Values.Count)
                        {
                            @params.Add(ParseParam(param, token.Values[index]));
                        }

                        arguments.Add(@params.ToArray());
                    }
                    else
                    {
                        arguments.Add(ParseParam(param, token.Values[index]));
                    }
                }

				method.Invoke(obj, arguments.ToArray());
			}
		}

        private static object ParseParam(ParameterInfo param, object value)
        {
            var type = param.ParameterType;
            if (type.IsArray)
            {
                var length = ((List<string>)value).Count;

                var array = Array.CreateInstance(type.GetElementType(), length);
                var source = ((List<string>)value).Select(x => Convert.ChangeType(x, type.GetElementType())).ToArray();

                Array.Copy(source, array, length);

                return array;
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, (string)value);
            }

            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                var genericArguments = type.GetGenericArguments();
                var listType = typeof(List<>).MakeGenericType(genericArguments);

                var list = Activator.CreateInstance(listType) as IList;

                var args = ( (IEnumerable<string>)value ).Select(x => Convert.ChangeType(x, genericArguments[0]));

                foreach (var arg in args)
                {
                    list?.Add(arg);
                }

                return list;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<,>) && type.GetGenericArguments().First().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INVQueryBuilder<>)))
            {
                var parameterType = type.GetGenericArguments()[0];
                var returnType = type.GetGenericArguments()[1];

                var parameter = Expression.Parameter(parameterType, "x");
                var applyMethod = typeof(QueryParamHelper).GetMethod("Apply");

                var expr = Expression.Lambda(type,
                    Expression.Block(type.GetGenericArguments()[1],
                        Expression.Call(applyMethod, parameter, Expression.Constant(value)),
                        parameter
                    ),
                    parameter
                );

                return expr.Compile();
            }

            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 
        /// </summary>
        private static class QueryParamTokenizer
		{
			public static IEnumerable<QueryParamToken> Tokenize(string input)
			{
				int offset = 0;
				int currentIndex = 0;
				int level = 0;
				bool addArray = false;
				var array = new List<string>();

				var token = new QueryParamToken();

				foreach (var c in input)
				{
					switch (c)
					{
						case '(':
							if (level == 0)
							{
								token.MethodName = input.Substring(offset, currentIndex - offset).Trim();
								offset = currentIndex + 1;
							}
							level++;
							break;
						case ')':
							level--;
							if (level == 0)
							{
								if (addArray)
								{
									token.Values.Add(array);
								}
								else
								{
                                    var value = input.Substring(offset, currentIndex - offset).Trim();
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        token.Values.Add(value);
                                    }
								}
								addArray = false;
								
								yield return token;

								token = new QueryParamToken();
								offset = currentIndex + 1;
							}
							break;
						case '.':
							if (level == 0)
							{
								offset = currentIndex + 1;
							}
							break;
						case ',':
							if (level == 1)
							{
								if (addArray)
								{
									array.Add(input.Substring(offset, currentIndex - offset).Trim());
								} else
								{
									token.Values.Add(input.Substring(offset, currentIndex - offset).Trim());
								}
								
								offset = currentIndex + 1;
							}
							break;
						case '[':
							if (level == 1)
							{
								array = new List<string>();
								addArray = true;

								offset = currentIndex + 1;
							}
							break;
						case ']':
							if (level == 1)
							{
								array.Add(input.Substring(offset, currentIndex - offset).Trim());

								offset = currentIndex + 1;
							}
							break;
					}

					currentIndex++;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private class QueryParamToken
		{
			/// <summary>
			/// Gets or sets the name of the method.
			/// </summary>
			/// <value>
			/// The name of the method.
			/// </value>
			public string MethodName { get; set; }

			/// <summary>
			/// Gets or sets the values.
			/// </summary>
			/// <value>
			/// The values.
			/// </value>
			public List<object> Values { get; set; } = new List<object>();
		}
	}
}