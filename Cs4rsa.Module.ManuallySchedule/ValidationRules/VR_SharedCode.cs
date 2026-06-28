using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;

using Newtonsoft.Json;

using System;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.ValidationRules
{
    /// <summary>
    /// This validation rule checks if the input string is a valid base64-encoded JSON representation of an array of UserSubject objects.
    /// 
    /// AXin - 2026/06/28
    /// </summary>
    internal class VR_SharedCode : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value as string;
            if (string.IsNullOrEmpty(input)) return ValidationResult.ValidResult;
            try
            {
                // Thử decode base64
                var encodedDataAsBytes = Convert.FromBase64String(input);
                var decodedString = Encoding.UTF8.GetString(encodedDataAsBytes);
                JsonConvert.DeserializeObject<UserSubject[]>(decodedString);
                return ValidationResult.ValidResult;
            }
            catch
            {
                return new ValidationResult(false, "Shared code is invalid.");
            }
        }
    }
}
