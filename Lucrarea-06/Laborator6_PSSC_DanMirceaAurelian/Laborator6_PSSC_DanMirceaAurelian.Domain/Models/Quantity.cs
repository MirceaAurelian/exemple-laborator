﻿using LanguageExt;
using static LanguageExt.Prelude;

namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    public record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidQuantityException($"{value} is an invalid quantity value.");
            }
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static Option<Quantity> TryParse(int quantity)
        {
            if (IsValid(quantity))
            {
                return Some<Quantity>(new(quantity));
            }
            else
            {
                return None;
            }
        }
        public static Option<Quantity> TryParse(string quantityString)
        {
            if (int.TryParse(quantityString, out int numericQuantity) && IsValid(numericQuantity))
            {
                return Some<Quantity>(new(numericQuantity));
            }
            else
            {
                return None;
            }
        }

        private static bool IsValid(int numericQuantity) => numericQuantity > 0;
    }
}
