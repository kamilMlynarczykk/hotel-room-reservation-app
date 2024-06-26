using System;
using System.ComponentModel.DataAnnotations;

public class FutureOrPresentDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime < DateTime.Today)
            {
                return new ValidationResult("The date cannot be in the past.");
            }
        }
        return ValidationResult.Success;
    }
}
