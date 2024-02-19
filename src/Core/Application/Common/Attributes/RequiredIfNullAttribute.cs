using System.ComponentModel.DataAnnotations;
namespace Travaloud.Application.Common.Attributes;
#pragma warning disable CS8603 // Possible null reference return.
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class RequiredIfNullAttribute : ValidationAttribute
{
    private readonly string[] _otherPropertyNames;

    public RequiredIfNullAttribute(params string[] otherPropertyNames)
    {
        _otherPropertyNames = otherPropertyNames;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            var otherPropertyValues = _otherPropertyNames
                .Select(propertyName => validationContext.ObjectInstance.GetType()
                    .GetProperty(propertyName)?.GetValue(validationContext.ObjectInstance))
                .ToList();

            if (otherPropertyValues.All(propertyValue => propertyValue == null))
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}
