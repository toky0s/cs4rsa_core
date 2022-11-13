using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Utils;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Validations.Rules
{
    public class ShareStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                ShareString shareString = (ShareString)((App)Application.Current).Container.GetService(typeof(ShareString));
                IEnumerable<UserSubject> result = shareString.GetSubjectFromShareString((string)value);
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
