using System.Linq;

namespace NVShop
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider target, bool inherits) where TAttribute : Attribute
        {
            return target.IsDefined(typeof(TAttribute), inherits);
        }

        public static IEnumerable<Type> FindBaseTypes(this Type type, TypeFilter filter, object filterCriteria)
        {
            var baseTypes = new List<Type>();
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (filter(baseType, filterCriteria))
                {
                    baseTypes.Add(baseType);
                }

                baseType = baseType.BaseType;
            }

            return baseTypes;
        }


        public static Type GetSequenceType(this Type type)
        {
            var sequenceType = TryGetSequenceType(type);
            if (sequenceType == null)
            {
                throw new ArgumentException();
            }

            return sequenceType;
        }

        public static Type TryGetSequenceType(this Type type)
            => type.TryGetElementType(typeof(IEnumerable<>))
               ?? type.TryGetElementType(typeof(IAsyncEnumerable<>));

        public static Type TryGetElementType(this Type type, Type interfaceOrBaseType)
        {
            if (!type.GetTypeInfo().IsGenericTypeDefinition)
            {
                var types = GetGenericTypeImplementations(type, interfaceOrBaseType).ToList();

                return types.Count == 1 ? types[0].GetTypeInfo().GenericTypeArguments.FirstOrDefault() : null;
            }

            return null;
        }

        public static IEnumerable<Type> GetGenericTypeImplementations(this Type type, Type interfaceOrBaseType)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericTypeDefinition)
            {
                return (interfaceOrBaseType.GetTypeInfo().IsInterface ? typeInfo.ImplementedInterfaces : type.GetBaseTypes())
                    .Union(new[] { type })
                    .Where(
                        t => t.GetTypeInfo().IsGenericType
                             && t.GetGenericTypeDefinition() == interfaceOrBaseType);
            }

            return Enumerable.Empty<Type>();
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            type = type.GetTypeInfo().BaseType;

            while (type != null)
            {
                yield return type;

                type = type.GetTypeInfo().BaseType;
            }
        }
    }
}