using System.ComponentModel.DataAnnotations;

namespace NBasis.Validation
{
    public class LimitedValues : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' is not a recognized value for {1}";

        readonly string[] _strings;

        readonly int[] _ints;

        readonly bool _allowNull;

        public LimitedValues(string[] stringValues, bool allowNull = false, string errorMessage = DefaultErrorMessage) : base(errorMessage)
        {
            _strings = stringValues ?? throw new ArgumentNullException(nameof(stringValues), "Must have string values");
            _allowNull = allowNull;
        }

        public LimitedValues(int[] intValues, bool allowNull = false, string errorMessage = DefaultErrorMessage) : base(errorMessage)
        {
            _ints = intValues ?? throw new ArgumentNullException(nameof(intValues), "Must have int values");
            _allowNull = allowNull;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((value == null) &&
                (_allowNull))
            {
                return null;
            }

            if (value != null)
            {
                // check values
                if ((value is int intValue) &&
                    (_ints.Contains(intValue)))
                {
                    return null;
                }

                if ((value is Guid guidValue) &&
                    (_strings.Any(s => s.Equals(guidValue.ToString(), StringComparison.CurrentCultureIgnoreCase))))
                {
                    return null;
                }

                if ((value is string stringValue) &&
                    (_strings.Contains(stringValue)))
                {
                    return null;
                }
            }

            return new ValidationResult(string.Format(ErrorMessageString, value?.ToString() ?? "null", validationContext.DisplayName), validationContext.DisplayName.Yield());
        }
    }
}
