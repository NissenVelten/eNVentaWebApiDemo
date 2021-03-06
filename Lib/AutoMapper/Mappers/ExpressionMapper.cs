﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper.Configuration;
using AutoMapper.Internal;
using AutoMapper.XpressionMapper.Extensions;
using static System.Linq.Expressions.Expression;

namespace AutoMapper.Mappers
{
    public class ExpressionMapper : IObjectMapper
    {
        private static TDestination Map<TSource, TDestination>(TSource expression, ResolutionContext context)
            where TSource : LambdaExpression
            where TDestination : LambdaExpression => context.Mapper.MapExpression<TDestination>(expression);

        private static readonly MethodInfo MapMethodInfo = typeof(ExpressionMapper).GetDeclaredMethod(nameof(Map));

        public bool IsMatch(TypePair context) => typeof(LambdaExpression).IsAssignableFrom(context.SourceType)
                                                 && context.SourceType != typeof(LambdaExpression)
                                                 && typeof(LambdaExpression).IsAssignableFrom(context.DestinationType)
                                                 && context.DestinationType != typeof(LambdaExpression);

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap, PropertyMap propertyMap, Expression sourceExpression, Expression destExpression, Expression contextExpression) => 
            Call(null, 
                MapMethodInfo.MakeGenericMethod(sourceExpression.Type, destExpression.Type), 
                sourceExpression, 
                contextExpression);

        internal class MappingVisitor : ExpressionVisitor
        {
            private IList<Type> _destSubTypes = new Type[0];

            private readonly IConfigurationProvider _configurationProvider;
            private readonly TypeMap _typeMap;
            private readonly Expression _oldParam;
            private readonly Expression _newParam;
            private readonly MappingVisitor _parentMappingVisitor;

            public MappingVisitor(IConfigurationProvider configurationProvider, IList<Type> destSubTypes)
                : this(configurationProvider, null, Parameter(typeof(Nullable)), Parameter(typeof(Nullable)), null, destSubTypes)
            {
            }

            internal MappingVisitor(IConfigurationProvider configurationProvider, TypeMap typeMap, Expression oldParam, Expression newParam, MappingVisitor parentMappingVisitor = null, IList<Type> destSubTypes = null)
            {
                _configurationProvider = configurationProvider;
                _typeMap = typeMap;
                _oldParam = oldParam;
                _newParam = newParam;
                _parentMappingVisitor = parentMappingVisitor;
                if (destSubTypes != null)
                    _destSubTypes = destSubTypes;
            }

            protected override Expression VisitConstant(ConstantExpression node) => ReferenceEquals(node, _oldParam) ? _newParam : node;

            protected override Expression VisitParameter(ParameterExpression node) => ReferenceEquals(node, _oldParam) ? _newParam : node;

            protected override Expression VisitMethodCall(MethodCallExpression node) {

                var convertedArguments = Visit(node.Arguments);

                if (!node.Method.IsGenericMethod)
                {
                    var source = Visit(node.Object);
                    var sourceType = source?.Type ?? node.Type;

                    var sourceMethod = sourceType.GetDeclaredMethods().FirstOrDefault(x => x.Name == node.Method.Name);
                    // Map Methods if it has the same name
                    if (sourceMethod != null)
                    {
                        var parameterExpressions = new List<Expression>();

                        var sourceParameters = sourceMethod.GetParameters();

                        for (int i = 0; i < sourceParameters.Length; i++)
                        {
                            if (convertedArguments[i].Type.CanImplicitConvert(sourceParameters[i].ParameterType))
                            {
                                parameterExpressions.Add(Convert(convertedArguments[i], sourceParameters[i].ParameterType));
                            }
                        }

                        var methodCall = Call(source, sourceMethod, parameterExpressions);
                        if (methodCall.Type != node.Type)
                        {
                            if (methodCall.Type.CanImplicitConvert(node.Type))
                            {
                                return Convert(methodCall, node.Type);
                            }
                        }
                    }

                    return node;
                }

                Type[] convertedMethodArgumentTypes;
                switch (node.Method.Name)
                {
                    case "Select":
                        convertedMethodArgumentTypes = node.Method.GetGenericArguments().Select((t, idx) => idx == 0 ? GetConvertingTypeIfExists(node.Arguments[idx], t, convertedArguments[idx]) : t).ToArray();
                        break;
                    default:
                        convertedMethodArgumentTypes = node.Method.GetGenericArguments().Select((t, idx) => GetConvertingTypeIfExists(node.Arguments[idx], t, convertedArguments[idx])).ToArray();
                        break;
                }

                var convertedMethodCall = node.Method.GetGenericMethodDefinition().MakeGenericMethod(convertedMethodArgumentTypes);

                return base.VisitMethodCall(Call(convertedMethodCall, convertedArguments));
            }

            protected override Expression VisitExtension(Expression node) => (int)node.NodeType == 10000 ? node : base.VisitExtension(node);

            private static Type GetConvertingTypeIfExists(Expression arg, Type t, Expression converted)
            {
                if (arg.Type.IsGenericType() && arg.Type == t)
                {
                    return converted.Type;
                }

                if (arg.Type.IsGenericType() && arg.Type.GetTypeInfo().GenericTypeArguments[0] == t)
                {
                    return converted.Type.GetTypeInfo().GenericTypeArguments[0];
                }

                var expr = arg as UnaryExpression;
                if (expr != null)
                {
                    var convertedExpr = converted as UnaryExpression;

                    var lambdaExpr = expr.Operand as LambdaExpression;
                    if (lambdaExpr != null && lambdaExpr.ReturnType == t)
                    {
                        return ((LambdaExpression)convertedExpr.Operand).ReturnType;
                    }
                }
                
                return t;
            }

            public override Expression Visit(Expression node)
            {
                return base.Visit(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var newLeft = Visit(node.Left);
                var newRight = Visit(node.Right);

                if (newLeft.Type != newRight.Type)
                {
                    if (ReflectionHelper.CanImplicitConvert(newRight.Type, newLeft.Type))
                    {
                        newRight = ExpressionFactory.ToType(newRight, newLeft.Type);
                    }
                    else if (ReflectionHelper.CanImplicitConvert(newLeft.Type, newRight.Type))
                    {
                        newLeft = ExpressionFactory.ToType(newLeft, newRight.Type);
                    }
                    else
                    {
                        var typeMap = _configurationProvider.ResolveTypeMap(newRight.Type, newLeft.Type);
                        if (typeMap != null)
                        {
                            var visitor = new MappingVisitor(_configurationProvider, typeMap, newRight, newLeft, this);
                            newRight = visitor.Visit(newRight);
                        }
                        else
                        {
                            // check if the non-string expression is a null constent
                            // as this would lead to a "null.ToString()" and thus an error when executing the expression
                            if (newLeft.Type != newRight.Type && newRight.Type == typeof(string) && !IsNullConstant(newLeft))
                                newLeft = Call(newLeft, typeof(object).GetDeclaredMethod("ToString"));
                            if (newRight.Type != newLeft.Type && newLeft.Type == typeof(string) && !IsNullConstant(newRight))
                                newRight = Call(newRight, typeof(object).GetDeclaredMethod("ToString"));
                            CheckNullableToNonNullableChanges(node.Left, node.Right, ref newLeft, ref newRight);
                            CheckNullableToNonNullableChanges(node.Right, node.Left, ref newRight, ref newLeft);
                        }
                    }
                }

                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        return MakeBinary(node.NodeType, newLeft, newRight, node.IsLiftedToNull, node.Method);
                    default:
                        return MakeBinary(node.NodeType, newLeft, newRight);
                }

                bool IsNullConstant(Expression expression) => expression is ConstantExpression constant && constant.Value == null;
            }


            private static void CheckNullableToNonNullableChanges(Expression left, Expression right, ref Expression newLeft, ref Expression newRight)
            {
                if (GoingFromNonNullableToNullable(left, newLeft))
                    if (BothAreNonNullable(right, newRight))
                        UpdateToNullableExpression(right, out newRight);
                    else if (BothAreNullable(right, newRight))
                        UpdateToNonNullableExpression(right, out newRight);

                if (GoingFromNonNullableToNullable(newLeft, left))
                    if (BothAreNonNullable(right, newRight))
                        UpdateToNullableExpression(right, out newRight);
                    else if (BothAreNullable(right, newRight))
                        UpdateToNonNullableExpression(right, out newRight);
            }

            private static void UpdateToNullableExpression(Expression right, out Expression newRight)
            {
                newRight = right is ConstantExpression expression
                    ? Constant(expression.Value,
                        typeof(Nullable<>).MakeGenericType(right.Type))
                    : throw new AutoMapperMappingException(
                        "Mapping a BinaryExpression where one side is nullable and the other isn't");
            }

            private static void UpdateToNonNullableExpression(Expression right, out Expression newRight)
            {
                if (right is ConstantExpression expression)
                {
                    var t = right.Type.IsNullableType()
                        ? right.Type.GetGenericArguments()[0]
                        : right.Type;
                    newRight = Constant(expression.Value, t);
                }
                else if (right is UnaryExpression)
                    newRight = ((UnaryExpression) right).Operand;
                else
                    throw new AutoMapperMappingException(
                        "Mapping a BinaryExpression where one side is nullable and the other isn't");
            }

            private static bool GoingFromNonNullableToNullable(Expression node, Expression newLeft) 
                => !node.Type.IsNullableType() && newLeft.Type.IsNullableType();

            private static bool BothAreNullable(Expression node, Expression newLeft) 
                => node.Type.IsNullableType() && newLeft.Type.IsNullableType();

            private static bool BothAreNonNullable(Expression node, Expression newLeft) 
                => !node.Type.IsNullableType() && !newLeft.Type.IsNullableType();

            protected override Expression VisitLambda<T>(Expression<T> expression)
            {
                return expression.Parameters.Any(b => b.Type == _oldParam.Type) 
                    ? VisitLambdaExpression(expression) 
                    : VisitAllParametersExpression(expression);
            }

            private Expression VisitLambdaExpression<T>(Expression<T> expression)
            {
                var convertedBody = Visit(expression.Body);
                var convertedArguments = expression.Parameters.Select(e => Visit(e) as ParameterExpression).ToList();
                return Lambda(convertedBody, convertedArguments);
            }

            private Expression VisitAllParametersExpression<T>(Expression<T> expression)
            {
                var visitors = (
                    from t in expression.Parameters
                    let sourceParamType = t.Type
                    from destParamType in _destSubTypes.Where(dt => dt != sourceParamType)
                    let a = destParamType.IsGenericType() ? destParamType.GetTypeInfo().GenericTypeArguments[0] : destParamType
                    let typeMap = _configurationProvider.ResolveTypeMap(a, sourceParamType)
                    where typeMap != null
                    let oldParam = t
                    let newParam = Parameter(a, oldParam.Name)
                    select new MappingVisitor(_configurationProvider, typeMap, oldParam, newParam, this))
                    .Cast<ExpressionVisitor>()
                    .ToList();

                return visitors.Aggregate(expression as Expression, (e, v) => v.Visit(e));
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node == _oldParam)
                {
                    return _newParam;
                }

                var propertyMap = PropertyMap(node);

                if (propertyMap == null)
                {
                    if (node.Expression is MemberExpression)
                        return GetConvertedSubMemberCall(node);

                    return node;
                } 

                var constantVisitor = new IsConstantExpressionVisitor();
                constantVisitor.Visit(node);
                if (constantVisitor.IsConstant)
                    return node;

                SetSourceSubTypes(propertyMap);

                var replacedExpression = Visit(node.Expression);
                if (replacedExpression == node.Expression)
                    replacedExpression = _parentMappingVisitor.Visit(node.Expression);

                if (propertyMap.CustomExpression != null)
                    return propertyMap.CustomExpression.ReplaceParameters(replacedExpression);

                Func<Expression, MemberInfo, Expression> getExpression = MakeMemberAccess;

                return propertyMap.SourceMembers
                    .Aggregate(replacedExpression, getExpression);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Convert)
                {
                    var replacingExpression = Visit(node.Operand);
                    if (replacingExpression.Type != node.Operand.Type)
                    {
                        var typeMap = _configurationProvider.FindTypeMapFor(replacingExpression.Type, node.Type);
                        if (typeMap != null && typeMap.CustomProjection != null)
                        {
                            replacingExpression = typeMap.CustomProjection.ConvertReplaceParameters(replacingExpression);
                        }
                        else if (replacingExpression.Type.CanImplicitConvert(node.Type))
                        {
                            replacingExpression = Convert(replacingExpression, node.Type);
                        }

                        return Convert(replacingExpression, node.Type);
                    }

                    //if (node.Type != node.Operand.Type)
                    //{
                    //    return Convert(node.Operand, node.Operand.Type);
                    //}

                    var call = node.Operand as MethodCallExpression;
                    if (call != null)
                    {
                        //var convertedMethod = GetConvertedMethodCall(call);

                        //if (convertedMethod.Type != node.Type)
                        //{
                        //    return Visit(Convert(convertedMethod, node.Type));
                        //}

                        //return Visit(convertedMethod);
                    }
                }

                return base.VisitUnary(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                var convertedArguments = node.Arguments.Select(GetConvertedArgument).ToList();

                return New(node.Constructor, convertedArguments);
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                var ifTrue = Visit(node.IfTrue);
                var ifFalse = Visit(node.IfFalse);

                if (ifTrue.Type != ifFalse.Type)
                {
                    var typeMap = _configurationProvider.FindTypeMapFor(ifFalse.Type, ifTrue.Type);
                    if (typeMap != null && typeMap.CustomProjection != null)
                    {
                        ifFalse = typeMap.CustomProjection.ConvertReplaceParameters(ifFalse);
                    }
                    else if (ifFalse.Type.CanImplicitConvert(ifTrue.Type))
                    {
                        ifFalse = Convert(ifFalse, ifTrue.Type);
                    }

                    return Condition(node.Test, ifTrue, ifFalse);
                }

                return base.VisitConditional(node);
            }

            protected override MemberBinding VisitMemberBinding(MemberBinding node)
            {
                return base.VisitMemberBinding(node);
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                var convertedExpression = Visit(node.Expression);
                return base.VisitMemberAssignment(node.Update(convertedExpression));
            }
                        
            private class TypeConvertingExpressionVisitor : ExpressionVisitor
            {
                protected override Expression VisitMember(MemberExpression node)
                {
                    return base.VisitMember(node);
                }
            }

            private class IsConstantExpressionVisitor : ExpressionVisitor
            {
                public bool IsConstant { get; private set; }

                protected override Expression VisitConstant(ConstantExpression node)
                {
                    IsConstant = true;

                    return base.VisitConstant(node);
                }
            }

            private Expression GetConvertedArgument(Expression node)
            {
                var baseExpression = Visit(node);

                if (node.Type == baseExpression.Type)
                    return baseExpression;

                var propertyMap = FindPropertyMapOfExpression(node as MemberExpression);
                if (propertyMap == null)
                {
                    return ExpressionFactory.ToType(baseExpression, node.Type);
                }

                var sourceType = GetSourceType(propertyMap);
                var destType = propertyMap.DestinationPropertyType;

                if (sourceType != destType)
                {
                    if (ReflectionHelper.CanImplicitConvert(sourceType, destType))
                    {
                        return ExpressionFactory.ToType(baseExpression, destType);
                    }
                }

                var typeMap = _configurationProvider.ResolveTypeMap(sourceType, destType);
                if (typeMap != null)
                {
                    var visitor = new MappingVisitor(_configurationProvider, typeMap, baseExpression, node, this);
                    return visitor.Visit(node);
                }
                return baseExpression;
            }
            
            private Expression GetConvertedSubMemberCall(MemberExpression node)
            {
                var baseExpression = Visit(node.Expression);
                var propertyMap = FindPropertyMapOfExpression(node.Expression as MemberExpression);
                if (propertyMap == null)
                    return node;
                var sourceType = GetSourceType(propertyMap);
                var destType = propertyMap.DestinationPropertyType;
                if (sourceType == destType)
                    return MakeMemberAccess(baseExpression, node.Member);
                var typeMap = _configurationProvider.ResolveTypeMap(sourceType, destType);
                var subVisitor = new MappingVisitor(_configurationProvider, typeMap, node.Expression, baseExpression, this);
                var newExpression = subVisitor.Visit(node);
                _destSubTypes = _destSubTypes.Concat(subVisitor._destSubTypes).ToArray();
                return newExpression;
            }

            private Type GetSourceType(PropertyMap propertyMap) =>
                propertyMap.SourceType ??
                throw new AutoMapperMappingException(
                    "Could not determine source property type. Make sure the property is mapped.", 
                    null, 
                    new TypePair(null, propertyMap.DestinationPropertyType), 
                    propertyMap.TypeMap, 
                    propertyMap);

            private PropertyMap FindPropertyMapOfExpression(MemberExpression expression)
            {
                var propertyMap = PropertyMap(expression);
                return propertyMap == null && expression.Expression is MemberExpression
                    ? FindPropertyMapOfExpression((MemberExpression) expression.Expression)
                    : propertyMap;
            }

            private PropertyMap PropertyMap(MemberExpression node)
                => _typeMap == null
                    ? null
                    : (node.Member.IsStatic()
                        ? null
                        : (!node.Member.DeclaringType.IsAssignableFrom(_typeMap.DestinationType)
                            ? null
                            : _typeMap.GetExistingPropertyMapFor(node.Member)));

            private void SetSourceSubTypes(PropertyMap propertyMap)
            {
                if (propertyMap.SourceMember is PropertyInfo info)
                    _destSubTypes = info.PropertyType.GetTypeInfo().GenericTypeArguments.Concat(new[] { info.PropertyType }).ToList();
                else if (propertyMap.SourceMember is FieldInfo fInfo)
                    _destSubTypes = fInfo.FieldType.GetTypeInfo().GenericTypeArguments;
            }
        }
    }
}
