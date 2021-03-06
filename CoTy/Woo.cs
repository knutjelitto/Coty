﻿using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using CoTy.Objects;
using CoTy.Support;

namespace CoTy
{
    public class Woo : IDynamicMetaObjectProvider
    {
        public static void Doo()
        {
            CallSiteBinder binder = new SimpleOperationBinder();

            var dynamicExpression = Expression.Dynamic(
                binder,
                typeof(object),
                new Expression[] {Expression.Constant(5), Expression.Constant(2)});

            var compileDelegate = Expression.Lambda<Func<object>>(dynamicExpression).Compile();

            var result = compileDelegate();

            Console.WriteLine($"result is {result}");
        }

        private class SimpleOperationBinder : BinaryOperationBinder
        {
            public SimpleOperationBinder() : base(ExpressionType.Add)
            {
            }

            public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
            {
                return new DynamicMetaObject(
                    Expression.Convert(Expression.Constant(3), typeof(object)),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true))
                    );
            }
        }

        public static void Doo3()
        {
            Func<dynamic, dynamic, object> binOp = (v1, v2) => v1 * v2;

            var scope = Expression.Parameter(typeof(IScope), "scope");
            var stack = Expression.Parameter(typeof(Stack), "stack");
            var value1 = Expression.Parameter(typeof(object), "value1");
            var value2 = Expression.Parameter(typeof(object), "value2");
            var result = Expression.Parameter(typeof(object), "result");
            var block = Expression.Block(
                new ParameterExpression[] { value1, value2, result },
                Expression.Assign(value2, Expression.Call(stack, "Pop", Type.EmptyTypes)),
                Expression.Assign(value1, Expression.Call(stack, "Pop", Type.EmptyTypes)),
                Expression.Assign(result, Expression.Call(Expression.Constant(binOp.Target), binOp.Method, value1, value2)),
                Expression.Call(stack, "Push", Type.EmptyTypes, result));
            var lambda = Expression.Lambda<Action<IScope, IStack>>(block, scope, stack);
            var action = lambda.Compile();

            var xcontext = Context.Root(Binder.From("root"));
            var xstack = Stack.From(Integer.From(10), Integer.From(10));

            action(xcontext, xstack);

            xstack.Push(Integer.From(10));
            xstack.Push(Integer.From(10));

            action(xcontext, xstack);
        }

        public static void Doo2()
        {
            var woo = new Woo();
            var chars1 = "a";
            var chars2 = "b";
            var int1 = Integer.From(11);
            var int2 = Integer.From(12);

            woo.Call(woo, chars1, chars2);
            woo.Call(woo, chars1, chars2);

            Environment.Exit(12);
        }

        private void Call(Woo woo, string chars1, string chars2)
        {
            ((dynamic)woo).Add(chars1, chars2);
        }

        public void Add(string value2, string value)
        {
        }

        public void Add(Integer value1, Integer value2)
        {
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new MyDynamicMetaObject(parameter, this);
        }

        private class MyDynamicMetaObject : DynamicMetaObject
        {
            private static Type voidType = typeof(void);

            public MyDynamicMetaObject(Expression expression, object value) : base(expression, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                var method = Value.GetType().GetMethod(binder.Name, args.Select(arg => arg.RuntimeType).ToArray());

                var defaulting = base.BindInvokeMember(binder, args);

                if (method == null)
                {
                    return defaulting;
                }

                var self = Expression.Convert(Expression, LimitType);
                // ReSharper disable once AssignNullToNotNullAttribute
                var parameters = args.Select(arg => Expression.Convert(arg.Expression, arg.RuntimeType));

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
