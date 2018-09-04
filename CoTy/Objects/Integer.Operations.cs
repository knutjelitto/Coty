﻿// ReSharper disable UnusedMember.Global

using Microsoft.Win32.SafeHandles;

namespace CoTy.Objects
{
    public partial class Integer
    {
        public Integer Add(Integer other)
        {
            return new Integer(Value + other.Value);
        }

        public CoTuple Add(dynamic other)
        {
            return other.CoAdd(this);
        }

        public Integer Sub(Integer other)
        {
            return new Integer(Value - other.Value);
        }

        public CoTuple Sub(dynamic other)
        {
            return other.CoSub(this);
        }

        public Integer Mul(Integer other)
        {
            return new Integer(Value * other.Value);
        }

        public CoTuple Mul(dynamic other)
        {
            return other.CoMul(this);
        }

        public Integer Div(Integer other)
        {
            return new Integer(Value / other.Value);
        }

        public CoTuple Div(dynamic other)
        {
            return other.CoDiv(this);
        }

        public Integer Succ()
        {
            return new Integer(Value + 1);
        }

        public Integer Pred()
        {
            return new Integer(Value - 1);
        }

        // equality

        public Bool Equal(Integer other)
        {
            return Value == other.Value;
        }

        // ordering

        public Bool Less(Integer other)
        {
            return Value < other.Value;
        }
    }
}