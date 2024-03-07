using NBasis.Validation;
using System.ComponentModel.DataAnnotations;

namespace NBasis.CoreTests.Validation
{
    public class GuidValidationTests
    {
        public class GuidCommand
        {
            [RequiredGuid]
            public Guid ReqGuid { get; set; }
        }

        public class NullableGuidCommand
        {
            [RequiredGuid]
            public Guid? ReqGuid { get; set; }
        }

        [Fact(DisplayName = "Guid is required succeeds")]
        public void Guid_is_required_succeeds()
        {
            var cmd = new GuidCommand
            {
                ReqGuid = Guid.NewGuid()
            };

            var validationContext = new ValidationContext(cmd, null, null);
            Validator.ValidateObject(cmd, validationContext, true);

            // just here for test to pass
            Assert.NotNull(cmd);
        }

        [Fact(DisplayName = "Guid is required fails")]
        public void Guid_is_required_fails()
        {
            var cmd = new GuidCommand();

            var validationContext = new ValidationContext(cmd, null, null);

            ValidationException ex = Assert.Throws<ValidationException>(() => Validator.ValidateObject(cmd, validationContext, true));

            Assert.NotNull(ex.ValidationResult);
            Assert.Equal("ReqGuid", ex.ValidationResult.MemberNames.First());
            Assert.Equal("'ReqGuid' is required", ex.ValidationResult.ErrorMessage);
        }

        [Fact(DisplayName = "Empty Guid is required fails")]
        public void Empty_Guid_is_required_fails()
        {
            var cmd = new GuidCommand()
            {
                ReqGuid = Guid.Empty
            };

            var validationContext = new ValidationContext(cmd, null, null);

            ValidationException ex = Assert.Throws<ValidationException>(() => Validator.ValidateObject(cmd, validationContext, true));

            Assert.NotNull(ex.ValidationResult);
            Assert.Equal("ReqGuid", ex.ValidationResult.MemberNames.First());
            Assert.Equal("'ReqGuid' is required", ex.ValidationResult.ErrorMessage);
        }

        [Fact(DisplayName = "Nullable Guid is required succeeds")]
        public void Nullable_Guid_is_required_succeeds()
        {
            var cmd = new NullableGuidCommand
            {
                ReqGuid = Guid.NewGuid()
            };

            var validationContext = new ValidationContext(cmd, null, null);
            Validator.ValidateObject(cmd, validationContext, true);

            // just here for test to pass
            Assert.NotNull(cmd);
        }

        [Fact(DisplayName = "Nullable Guid is required fails")]
        public void Nullable_Guid_is_required_fails()
        {
            var cmd = new NullableGuidCommand();

            var validationContext = new ValidationContext(cmd, null, null);

            ValidationException ex = Assert.Throws<ValidationException>(() => Validator.ValidateObject(cmd, validationContext, true));

            Assert.NotNull(ex.ValidationResult);
            Assert.Equal("ReqGuid", ex.ValidationResult.MemberNames.First());
            Assert.Equal("'ReqGuid' is required", ex.ValidationResult.ErrorMessage);
        }

        [Fact(DisplayName = "Empty Nullable Guid is required fails")]
        public void Empty_nullable_Guid_is_required_fails()
        {
            var cmd = new NullableGuidCommand()
            {
                ReqGuid = Guid.Empty
            };

            var validationContext = new ValidationContext(cmd, null, null);

            ValidationException ex = Assert.Throws<ValidationException>(() => Validator.ValidateObject(cmd, validationContext, true));

            Assert.NotNull(ex.ValidationResult);
            Assert.Equal("ReqGuid", ex.ValidationResult.MemberNames.First());
            Assert.Equal("'ReqGuid' is required", ex.ValidationResult.ErrorMessage);
        }
    }
}