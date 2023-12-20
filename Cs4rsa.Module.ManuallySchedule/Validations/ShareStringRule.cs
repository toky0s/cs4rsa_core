using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.Validations
{
    public class ShareStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var result = ShareString.GetSubjectFromShareString((string)value);
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
