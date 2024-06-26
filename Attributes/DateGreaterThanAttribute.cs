using System;
using System.ComponentModel.DataAnnotations;

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _startDatePropertyName;

    public DateGreaterThanAttribute(string startDatePropertyName)
    {
        _startDatePropertyName = startDatePropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var endDate = (DateTime)value;
        var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
        if (startDateProperty == null)
        {
            return new ValidationResult($"Unknown property: {_startDatePropertyName}");
        }

        var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);

        if (endDate <= startDate)
        {
            return new ValidationResult(ErrorMessage ?? "End date must be greater than start date");
        }

        return ValidationResult.Success;
    }
}
