using NBasis.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBasis.CoreTests.Validation
{
    public class LimitedValuesValidationTests
    {
        public class LimitedStringCommand
        {
            [LimitedValues(new[] { "one", "two", "three" })]
            public string Val { get; set; }
        }

        public class LimitedIntCommand
        {
            [LimitedValues(new[] { 1, 2, 3 })]
            public int Val { get; set; }
        }

        public class LimitedNullableIntCommand
        {
            [LimitedValues(new[] { 1, 2, 3 }, true)]
            public int? Val { get; set; }
        }

        [Fact(DisplayName = "Limited string validation exception is valid")]
        public void Limited_string_validation_exception_is_valid()
        {
            var cmd = new LimitedStringCommand
            {
                Val = "five"
            };

            var validationContext = new ValidationContext(cmd, null, null);
            ValidationException ex = Assert.Throws<ValidationException>(() => Validator.ValidateObject(cmd, validationContext, true));

            Assert.NotNull(ex.ValidationResult);
            Assert.Equal("Val", ex.ValidationResult.MemberNames.First());
            Assert.Equal("'five' is not a recognized value for Val", ex.ValidationResult.ErrorMessage);
        }

        [Fact(DisplayName = "Limited string validation succeeds")]
        public void Limited_string_validation_succeeds()
        {
            var cmd = new LimitedStringCommand
            {
                Val = "two"
            };

            var validationContext = new ValidationContext(cmd, null, null);
            Validator.ValidateObject(cmd, validationContext, true);

            // just here for test to pass
            Assert.NotNull(cmd);
        }

        [Fact(DisplayName = "Limited string validation fails on null")]
        public void Limited_string_validation_fails_on_null()
        {
            var cmd = new LimitedStringCommand
            {
                Val = null
            };

            var validationContext = new ValidationContext(cmd, null, null);
            ValidationException ex = Assert.Throws<ValidationException>(() => Validator.ValidateObject(cmd, validationContext, true));

            Assert.NotNull(ex.ValidationResult);
            Assert.Equal("Val", ex.ValidationResult.MemberNames.First());
            Assert.Equal("'null' is not a recognized value for Val", ex.ValidationResult.ErrorMessage);
        }

        [Fact(DisplayName = "Limited int validation succeeds")]
        public void Limited_int_validation_succeeds()
        {
            var cmd = new LimitedIntCommand
            {
                Val = 2
            };

            var validationContext = new ValidationContext(cmd, null, null);
            Validator.ValidateObject(cmd, validationContext, true);

            // just here for test to pass
            Assert.NotNull(cmd);
        }
    }
}
