using System;
using System.Text.RegularExpressions;

namespace ShutdownController.Utility
{
    public static class ParseInputBox
    {
        public static bool IsOnlyNumber(string text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            Regex regex = new Regex(Properties.ConstTemplates.RegexForNumbers);

            return regex.IsMatch(text);
        }

        public static bool IsOnlyNumberOrPoint(string text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            Regex regex = new Regex(Properties.ConstTemplates.RegexForNumberAndPoint);

            return regex.IsMatch(text);
        }
    }
}
