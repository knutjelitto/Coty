﻿// ReSharper disable UnusedMember.Global

using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using CoTy.Errors;

namespace CoTy.Objects
{
    public partial class Cobject : IDynamicMetaObjectProvider
    {
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new Dynamic(parameter, this);
        }

        private class Dynamic : DynamicMetaObject
        {
            private static readonly Type voidType = typeof(void);

            public Dynamic(Expression expression, object value) : base(expression, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                var method = Value.GetType().GetMethod(binder.Name, args.Select(arg => arg.LimitType).ToArray());

                var defaulting = base.BindInvokeMember(binder, args);

                if (method == null)
                {
                    var error = Expression.Throw(
                        Expression.Constant(new DynaException(binder.Name, binder.ReturnType, args.Select(arg => arg.LimitType).ToArray())),
                        binder.ReturnType);

                    var notFound = new DynamicMetaObject(
                        error,
                        defaulting.Restrictions);

                    return notFound;
                }

                var self = Expression.Convert(Expression, LimitType);
                var parameters = args.Select(arg => Expression.Convert(arg.Expression, arg.LimitType));

                Expression call = Expression.Call(self, method, parameters);

                if (binder.ReturnType != voidType && method.ReturnType == voidType)
                {
                    call = Expression.Block(call, Expression.Default(binder.ReturnType));
                }
                else if (binder.ReturnType != method.ReturnType)
                {
                    call = Expression.Convert(call, binder.ReturnType);
                }

                var methodInfo = new DynamicMetaObject(
                    call,
                    defaulting.Restrictions);

                return methodInfo;
            }
        }
    }
}
