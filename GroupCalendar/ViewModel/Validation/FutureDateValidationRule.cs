using GroupCalendar.ViewModel.Wrapper;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace GroupCalendar.ViewModel.Validation
{
    public class FutureDateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var date = (DateTime)value;
            if (0 > date.CompareTo(StartDateWrapper.StartDate))
            {
                var isValid = false;
                var error = "error";

                return new ValidationResult(isValid, error);
            }
            return ValidationResult.ValidResult;
        }

        private StartDateWrapper StartDateWrapper;
    }
}
