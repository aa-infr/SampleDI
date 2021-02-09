using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Entity
{
    public class MultilingualString
    {
        private readonly Dictionary<string, Func<string>> _languageMap;

        public MultilingualString()
        {
            _languageMap = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "FR", GetFrench },
                { "NL", GetDutch },
                { "EN", GetEnglish }
            };
        }

        public string French { get; set; } = string.Empty;
        public string Dutch { get; set; } = string.Empty;
        public string English { get; set; } = string.Empty;

        private string GetFrench() => French;

        private string GetDutch() => Dutch;

        private string GetEnglish() => English;

        public string GetString(string twoLettersIsoCode)
        {
            var result = string.Empty;

            if (_languageMap.ContainsKey(twoLettersIsoCode))
                result = _languageMap[twoLettersIsoCode]();
            return result;
        }

        public static readonly MultilingualString None = new MultilingualString();
    }
}