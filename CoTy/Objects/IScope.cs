﻿using System.Collections.Generic;

namespace CoTy.Objects
{
    public interface IScope
    {
        IScope Chain(IBinder binder);

        void Define(Symbol symbol, object value, bool isSealed = false, bool isOpaque = false);
        Binding Find(Symbol symbol);
        void GetValue(Symbol symbol, out object value);
        bool IsDefined(Symbol symbol);
        bool TryFind(Symbol symbol, out Binding binding);
        void Undefine(Symbol symbol);
        void Update(Symbol symbol, object value);

        IEnumerable<IBinder> Binders { get; }

    }
}