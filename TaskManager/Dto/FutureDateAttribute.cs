using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto;

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || value is DateTime date && date > DateTime.Now) // LOCAL TIME
        {
            return ValidationResult.Success!;
        }

        return new ValidationResult($"The {validationContext.DisplayName} field must be a future date.");
    }
}
