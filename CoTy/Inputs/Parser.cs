﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoTy.Errors;
using CoTy.Objects;

namespace CoTy.Inputs
{
    public class Parser : IEnumerable<CoTuple>
    {
        public readonly Scanner Scanner;

        public Parser(Scanner scanner)
        {
            this.Scanner = scanner;
        }

        public IEnumerator<CoTuple> GetEnumerator()
        {
            var current = new Cursor<CoTuple>(new ObjectSource(this.Scanner));
            var queue = new Queue<CoTuple>(2);

            while (current)
            {
                ParseObject(queue, ref current);
                while (queue.TryDequeue(out var obj))
                {
                    yield return obj;
                }
            }
        }

        private void ParseObject(Queue<CoTuple> queue, ref Cursor<CoTuple> current)
        {
            if (current.Item is CoSymbol symbol)
            {
                if (symbol == CoSymbol.LeftParent)
                {
                    ParseQuote(queue, ref current);
                    return;
                }
                if (symbol == CoSymbol.RightParent)
                {
                    throw new ParserException("ill: unbalanced ')' in input");
                }

                if (symbol == CoSymbol.Quoter)
                {
                    current = current.Next;
                    if (!current)
                    {
                        throw new ParserException($"ill: dangling {CoSymbol.Quoter} at end of input");
                    }
                    ParseObject(queue, ref current);
                    var quotation = new CoQuotation(queue.ToList());
                    queue.Clear();
                    queue.Enqueue(quotation);
                    return;
                }
                if (symbol.Value.Length > 1)
                {
                    switch (symbol.Value[0])
                    {
                        case ':':
                            queue.Enqueue(new CoString(symbol.Value.Substring(1)));
                            queue.Enqueue(CoSymbol.Define);
                            current = current.Next;
                            return;
                    }
                }
            }

            queue.Enqueue(current.Item);
            current = current.Next;
        }

        private void ParseQuote(Queue<CoTuple> queue, ref Cursor<CoTuple> current)
        {
            current = current.Next;
            var inner = new Queue<CoTuple>();

            while (current && current.Item != CoSymbol.RightParent)
            {
                ParseObject(inner, ref current);
            }
            if (!current)
            {
                throw new ParserException("ill: unbalanced '(' in input");
            }

            queue.Enqueue(new CoQuotation(inner));
            current = current.Next;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}