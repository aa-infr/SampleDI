using Infrabel.ICT.Framework.Entity;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrabel.ICT.Framework.Extension
{
    public static class StringExtensions
    {
        public static readonly char DotSeparator = '.';
        public static readonly char KebabCaseSeparator = '-';
        public static readonly char SnakeCaseSeparator = '_';
        private static readonly Lazy<Regex> CleanFilePathRegex = new Lazy<Regex>(() => new Regex($"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}]", RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> CleanFileNameRegex = new Lazy<Regex>(() => new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> MacExtractRegex = new Lazy<Regex>(() => new Regex(@"[A-F\d]{2}", RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> MacValidationRegex = new Lazy<Regex>(() => new Regex(@"^([A-F\d]{2}[-:\.]?){5}[A-F\d]{2}$", RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> SHA256Regex = new Lazy<Regex>(() => new Regex("^[a-f0-9]{64}$", RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> UserInformationExtractor = new Lazy<Regex>(() => new Regex(@"^(?>.+[\\\/]+)?(?>(?<UserName>.+))$"));

        public static string ExtractUserName(this string fullUserName)
        {
            if (string.IsNullOrWhiteSpace(fullUserName))
                return string.Empty;

            var match = UserInformationExtractor.Value.Match(fullUserName);
            if (!match.Success)
                return string.Empty;

            return match.Groups["UserName"].Value.ToUpper();
        }

        public static string FormatMacAddress(this string value)
        {
            return !IsMacAddressValid(value)
                ? string.Empty
                : string.Join(":", MacExtractRegex.Value.Matches(value).OfType<Match>().Select(m => m.Value));
        }

        public static bool IsMacAddressValid(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && MacValidationRegex.Value.IsMatch(value);
        }

        public static bool IsValidIpV4Format(this string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return false;

            var bytes = ipAddress.Split('.');
            return bytes.Length == 4 && bytes.All(e => byte.TryParse(e, out _));
        }

        public static bool IsValidSHA256(this string sha256Hash)
        {
            return !string.IsNullOrWhiteSpace(sha256Hash) && SHA256Regex.Value.IsMatch(sha256Hash);
        }

        public static string RemoveFirstOccurrence(this string value, string toRemove, bool caseSensitive = false)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(toRemove))
                return value;

            var index = value.IndexOf(toRemove, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

            return index < 0 ? value : value.Remove(index, toRemove.Length);
        }

        public static string ReplaceIllegalFileNameCharacters(this string value, string replacement = "")
        {
            return CleanFileNameRegex.Value.Replace(value ?? string.Empty, replacement);
        }

        public static bool IsValidFileName(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && !CleanFileNameRegex.Value.IsMatch(value);
        }

        public static bool IsValidFilePath(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && !CleanFilePathRegex.Value.IsMatch(value);
        }

        public static string SafeTrim(this string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
        }

        public static string ToAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sb = new StringBuilder();
            for (var i = 0; i < value.Length; i++)
            {
                var character = value[i];
                if (char.IsLetterOrDigit(character))
                    sb.Append(character);
            }

            return sb.ToString();
        }

        public static bool IsValidEmail(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                var address = new MailAddress(value);
                return string.IsNullOrEmpty(address.DisplayName);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static Guid ToGuid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Guid.Empty;

            return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
        }

        public static string ToLowerSnakeCase(this string value)
        {
            return ToSnakeCase(value, char.ToLower);
        }

        public static string ToSearchable(this string value, SearchableType type = SearchableType.None, string replacementString = " ")
        {
            if (value == null) return null;
            var result = new StringBuilder();

            switch (type)
            {
                // extract only digit
                case SearchableType.Digit:
                    foreach (var c in value)
                        if (char.IsDigit(c)) result.Append(c);
                    while (result.Length > 1 && result[0] == '0') result.Remove(0, 1);
                    break;
                // extract only digit or Letter
                case SearchableType.LetterOrDigit:
                    foreach (var c in value) if (char.IsLetterOrDigit(c)) result.Append(c);
                    return ToSearchable(result.ToString());

                case SearchableType.LetterOrDigitWithReplacement:
                    foreach (var c in ToSearchable(value))
                        if (char.IsLetterOrDigit(c)) result.Append(c);
                        else result.Append(replacementString);
                    break;

                default:
                    foreach (var c in value.Normalize(NormalizationForm.FormD))
                        if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                            result.Append(char.ToUpper(c));
                    break;
            }
            return result.ToString();
        }

        public static string ToUpperSnakeCase(this string value)
        {
            return ToSnakeCase(value, char.ToUpper);
        }

        public static string TrimToLower(this string value)
        {
            return value.SafeTrim().ToLower();
        }

        public static string TrimToLowerInvariant(this string value)
        {
            return value.SafeTrim().ToLowerInvariant();
        }

        public static string TrimToUpper(this string value)
        {
            return value.SafeTrim().ToUpper();
        }

        public static string TrimToUpperInvariant(this string value)
        {
            return value.SafeTrim().ToUpperInvariant();
        }

        public static bool TryExtractFirstDigits(this string ipAddress, out int firstDigits)
        {
            firstDigits = 0;
            if (!IsValidIpV4Format(ipAddress)) return false;
            var bytes = ipAddress.Split('.');
            if (!byte.TryParse(bytes[0], out var conversion)) return false;
            firstDigits = conversion;
            return true;
        }

        public static bool TryToFormatIpV4(this string ipAddress, out string formattedString)
        {
            formattedString = null;
            if (!IsValidIpV4Format(ipAddress)) return false;
            var bytes = ipAddress.Split('.');
            var result = new StringBuilder(ipAddress.Length);
            foreach (var digit in bytes)
            {
                result.Append(byte.Parse(digit));
                result.Append('.');
            }
            --result.Length;
            formattedString = result.ToString();
            return true;
        }

        private static bool IsCasingUniform(string value)
        {
            var lowerCaseCount = 0;
            var upperCaseCount = 0;

            foreach (var character in value)
            {
                if (char.IsUpper(character))
                    ++upperCaseCount;
                else if (char.IsLower(character))
                    ++lowerCaseCount;

                if (lowerCaseCount > 0 && upperCaseCount > 0)
                    return false;
            }
            return true;
        }

        private static string ToSnakeCase(this string value, Func<char, char> casingFunc)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();
            var isCasingUniform = IsCasingUniform(value);
            var sb = new StringBuilder();

            char lastAppendedChar = default;

            for (var i = 0; i < value.Length; i++)
            {
                var character = value[i];
                if (!TryNormalize(character, SnakeCaseSeparator, out var converted, casingFunc))
                    continue;
                if (converted == SnakeCaseSeparator && (lastAppendedChar == SnakeCaseSeparator || lastAppendedChar == default))
                    continue;

                if (!isCasingUniform && char.IsUpper(character) && lastAppendedChar != SnakeCaseSeparator && lastAppendedChar != default)
                    sb.Append(SnakeCaseSeparator);

                lastAppendedChar = converted;

                sb.Append(converted);
            }

            return lastAppendedChar == SnakeCaseSeparator ? sb.ToString(0, sb.Length - 1) : sb.ToString();
        }

        private static bool TryNormalize(char value, char separator, out char result, Func<char, char> casingFunc)
        {
            var normalized = true;
            if (char.IsLetter(value))
            {
                result = casingFunc(value);
            }
            else if (char.IsDigit(value))
            {
                result = value;
            }
            else if (char.IsWhiteSpace(value) || value == DotSeparator || value == KebabCaseSeparator || value == SnakeCaseSeparator)
            {
                result = separator;
            }
            else
            {
                result = value;
                normalized = false;
            }

            return normalized;
        }
    }
}