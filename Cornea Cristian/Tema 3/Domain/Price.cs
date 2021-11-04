using LanguageExt;
using static LanguageExt.Prelude;
using System;

namespace Laborator3_PSSC.Domain
{
    public record Price
    {
        public decimal Value { get; }

        public Price(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidPriceException($"{value:0.##} is an invalid price value.");
            }
        }

        public static Price operator *(Price a, Quantity b) => new Price((a.Value * b.Value));

        public Price Round()
        {
            var roundedValue = Math.Round(Value);
            return new Price(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static Option<Price> TryParse(string priceString)
        {
            if (decimal.TryParse(priceString, out decimal numericPrice) && IsValid(numericPrice))
            {
                return Some<Price>(new(numericPrice));
            }
            else
            {
                return None;
            }
        }

        private static bool IsValid(decimal numericPrice) => numericPrice >= 0;
    }
}
