using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Validations.Rules
{
    public class ShareStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
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
            catch (Exception e)
            {
                return new ValidationResult(false, $"Phân tích share string bị lỗi {e.Message}");
            }
        }
    }
}
