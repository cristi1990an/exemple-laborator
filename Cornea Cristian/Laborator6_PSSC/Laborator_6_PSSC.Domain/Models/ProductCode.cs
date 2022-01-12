using LanguageExt;
using static LanguageExt.Prelude;
using System.Text.RegularExpressions;

namespace Laborator_6_PSSC.Domain.Models
{
    public record ProductCode
    {
	    public const string Pattern = "^.*$";
        private static readonly Regex PatternRegex = new(Pattern);

        public string Code { get; }

        internal ProductCode(string value)
        {
            if (IsValid(value))
            {
                Code = value;
            }
            else
            {
                throw new InvalidProductCodeException("");
            }
        }

        private static bool IsValid(string stringValue) => PatternRegex.IsMatch(stringValue);

	public override string ToString()
	{
		return Code;
	}

        public static Option<ProductCode> TryParse(string productCodeString)
        {
            if (IsValid(productCodeString))
            {
                return Some<ProductCode>(new(productCodeString));
            }
            else
            {
                return None;
            }
        }
    }
}
