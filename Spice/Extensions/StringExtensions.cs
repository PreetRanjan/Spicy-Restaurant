using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Spice.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string name)
        {
            var str = name.ToLowerInvariant();
            str.Trim('-', '_');
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            var bytes = Encoding.GetEncoding("utf-8").GetBytes(str);
            str = Encoding.UTF8.GetString(bytes);

            str = Regex.Replace(str, @"\s", "-", RegexOptions.Compiled);
            str = Regex.Replace(str, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            str = RemoveIllegalCharacters(str);

            return str;
        }

        static string RemoveIllegalCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string[] chars = new string[] {
                ":", "/", "?", "!", "#", "[", "]", "{", "}", "@", "*", ".", ",",
                "\"","&", "'", "~", "$"
            };

            foreach (var ch in chars)
            {
                text = text.Replace(ch, string.Empty);
            }

            text = text.Replace("–", "-");
            text = text.Replace(" ", "-");

            text = RemoveUnicodePunctuation(text);
            text = RemoveDiacritics(text);
            text = RemoveExtraHyphen(text);

            return HttpUtility.HtmlEncode(text).Replace("%", string.Empty);
        }

        static string RemoveUnicodePunctuation(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in
                normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.InitialQuotePunctuation &&
                                      CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.FinalQuotePunctuation))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in
                normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        static string RemoveExtraHyphen(string text)
        {
            if (text.Contains("--"))
            {
                text = text.Replace("--", "-");
                return RemoveExtraHyphen(text);
            }

            return text;
        }

        public static string SanitizePath(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            str = str.Replace("%2E", ".").Replace("%2F", "/");

            if (str.Contains("..") || str.Contains("//"))
                throw new ApplicationException("Invalid directory path");

            return str;
        }

        public static string SanitizeFileName(this string str)
        {
            str = str.SanitizePath();

            //TODO: add filename specific validation here

            return str;
        }
    }
}
