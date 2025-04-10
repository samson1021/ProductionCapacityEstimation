using System.ComponentModel.DataAnnotations;

namespace mechanical.Validation
{
    public class RequiredEnumAttribute:ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Check if the value is null or the default value for the enum
            if (value == null || value.Equals(GetDefaultValue(value.GetType())))
            {
                return new ValidationResult($"The {validationContext.DisplayName} field is required.");
            }

            return ValidationResult.Success;
        }

        private object GetDefaultValue(Type type)
        {
            // Get the default value for the type (e.g., 0 for enums)
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
