using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Utils;

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace cs4rsa_core.Validations.Rules
{
    public class ShareStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            IEnumerable<UserSubject> result = ShareString.GetSubjectFromShareString((string)value);
            if (result == null)
            {
                return new ValidationResult(false, $"Share string không hợp lệ");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
