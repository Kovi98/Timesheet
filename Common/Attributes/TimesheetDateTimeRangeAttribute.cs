using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Timesheet.Common.Attributes
{
    public class TimesheetDateTimeRangeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string propertyNameToCheck;

        public TimesheetDateTimeRangeAttribute(string propertyNameToCheck)
        {
            this.propertyNameToCheck = propertyNameToCheck;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = "Čas Do nemůže být menší než čas Od",
                ValidationType = "TimesheetDateTimeRange"
            };
            rule.ValidationParameters.Add("valuetocheck", propertyNameToCheck);
            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyName = validationContext.ObjectType.GetProperty(propertyNameToCheck);
            if (propertyName == null)
                return new ValidationResult(string.Format(CultureInfo.CurrentCulture, "Unknown property {0}", new[] { propertyNameToCheck }));

            var propertyValue = propertyName.GetValue(validationContext.ObjectInstance, null) as DateTime?;

            var isNotNull = value != null && propertyValue != null;

            if (isNotNull && ((DateTime?)value < propertyValue))
                return new ValidationResult("Čas Do nemůže být menší než čas Od");

            return ValidationResult.Success;
        }
    }
}
