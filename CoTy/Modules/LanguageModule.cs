﻿using CoTy.Ambiance;
using CoTy.Objects;

namespace CoTy.Modules
{
    public class LanguageModule : Module
    {
        public LanguageModule(AmScope parent) : base(parent)
        {
        }

        [Builtin("define", ":")]
        private static void Define(AmScope scope, AmStack stack)
        {
            var toDefine = stack.Pop();
            var definition = stack.Pop();

            Symbol symbol;

            if (toDefine is Chars str)
            {
                symbol = Symbol.Get(str.Value);
            }
            else
            {
                symbol = (Symbol)toDefine;
            }

            scope.Define(symbol, definition);
        }

        [Builtin("eval")]
        private static void Eval(AmScope scope, AmStack stack)
        {
            var value = stack.Pop();
            value.Eval(scope, stack);
        }

        [Builtin("quote")]
        private static void Quote(AmScope scope, AmStack stack)
        {
            var @object = stack.Pop();
            var quotation = new Quotation(@object);
            stack.Push(quotation);
        }

        [Builtin("dequote")]
        private static void DeQuote(AmScope scope, AmStack stack)
        {
            foreach (var value in stack.Pop())
            {
                stack.Push(value);
            }
        }

        [Builtin("if")]
        private static void If(AmScope scope, AmStack stack)
        {
            var ifElse = stack.Pop();
            var ifTrue = stack.Pop();
            var condition = stack.Pop();

            condition.Eval(scope, stack);
            var result = stack.Pop();

            if (result is Bool boolean && boolean.Value)
            {
                ifTrue.Eval(scope, stack);
            }
            else
            {
                ifElse.Eval(scope, stack);
            }
        }

        [Builtin("when", Arity = 2)]
        private static void When(AmScope scope, AmStack stack)
        {
            var ifElse = stack.Pop();
            var ifTrue = stack.Pop();
            var condition = stack.Pop();

            condition.Eval(scope, stack);
            var result = stack.Pop();

            if (result is Bool boolean && boolean.Value)
            {
                ifTrue.Eval(scope, stack);
            }
            else
            {
                ifElse.Eval(scope, stack);
            }
        }

        [Builtin("unless", Arity = 2)]
        private static void Unless(AmScope scope, AmStack stack)
        {
            var guarded = stack.Pop();
            var condition = stack.Pop();

            condition.Apply(scope, stack);
            var result = stack.Pop();

            if (result is Bool boolean && !boolean.Value)
            {
                guarded.Apply(scope, stack);
            }
        }

        [Builtin("reduce")]
        private static void Reduce(AmScope scope, AmStack stack)
        {
            var operation = stack.Pop();
            var values = stack.Pop();

            var first = true;
            foreach (var value in values)
            {
                stack.Push(value);
                if (!first)
                {
                    operation.Eval(scope, stack);
                }
                else
                {
                    first = false;
                }
            }
        }
    }
}
