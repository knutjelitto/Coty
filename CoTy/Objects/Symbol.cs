﻿using System.Collections.Generic;

namespace CoTy.Objects
{
    public sealed class Symbol : Cobject<string>
    {
        private static readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        public static readonly Symbol Def = Get("def");
        public static readonly Symbol Quoter = Get("'");
        public static readonly Symbol BindTo = Get("=:");
        public static readonly Symbol ToBind = Get(":=");
        public static readonly Symbol Assign = Get("=>");
        public static readonly Symbol LeftParent = Get("(");
        public static readonly Symbol RightParent = Get(")");
        //public static readonly Symbol LeftBrace = Get("{");
        //public static readonly Symbol RightBrace = Get("}");

        private readonly int hashCode;

        private Symbol(string value) : base(value)
        {
            this.hashCode = value.GetHashCode();
        }

        public static implicit operator Symbol(string name)
        {
            return Get(name);
        }

        public static Symbol Get(string name)
        {
            if (!symbols.TryGetValue(name, out var symbol))
            {
                symbol = new Symbol(name);
                symbols.Add(name, symbol);
            }

            return symbol;
        }

        public override void Eval(IScope scope, IStack stack)
        {
            scope.GetValue(this, out var value);

            value.Eval(scope, stack);
        }

        public override int GetHashCode() => this.hashCode;

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Symbol other && Value == other.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
