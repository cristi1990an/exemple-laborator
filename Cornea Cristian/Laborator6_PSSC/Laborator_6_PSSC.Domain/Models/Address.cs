using LanguageExt;
using static LanguageExt.Prelude;

using System.Text.RegularExpressions;

namespace Laborator_6_PSSC.Domain.Models
{
    public record Address
    {
	    public const string Pattern = "^.*$";
        private static readonly Regex PatternRegex = new(Pattern);

        public string _address { get; }

        internal Address(string address)
        {
            if (IsValid(address))
            {
                _address = address;
            }
            else
            {
                throw new InvalidAddressException("");
            }
        }

	private static bool IsValid(string stringValue) => PatternRegex.IsMatch(stringValue);

        public override string ToString()
        {
            return _address;
        }

        public static Option<Address> TryParse(string addressString)
        {
            if (IsValid(addressString))
            {
                return Some<Address>(new(addressString));
            }
            else
            {
                return None;
            }
        }

    }
}
