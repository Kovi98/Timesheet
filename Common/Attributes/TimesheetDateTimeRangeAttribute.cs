using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Timesheet.Common.Attributes
{
    public class TimesheetDateTimeRangeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _otherProperty;

        public TimesheetDateTimeRangeAttribute(string propertyValueToCheck)
        {
            this._otherProperty = propertyValueToCheck;
            ErrorMessage = "Čas Do nemůže být menší než čas Od";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-datetimerange", ErrorMessage);
            context.Attributes.Add("data-val-datetimerange-otherproperty", $"TimesheetDetail_{_otherProperty}");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyName = validationContext.ObjectType.GetProperty(_otherProperty);
            if (propertyName == null)
                return new ValidationResult(string.Format(CultureInfo.CurrentCulture, "Unknown property {0}", new[] { _otherProperty }));

            var propertyValue = propertyName.GetValue(validationContext.ObjectInstance, null) as DateTime?;

            var isNotNull = value != null && propertyValue != null;

            if (isNotNull && ((DateTime?)value < propertyValue))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
