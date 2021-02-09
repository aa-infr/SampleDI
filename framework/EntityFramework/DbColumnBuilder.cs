using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    internal class DbColumnBuilder
    {
        private static readonly Dictionary<DataBaseProvider, char> DelimiterMap = new Dictionary<DataBaseProvider, char>()
        {
            {DataBaseProvider.Oracle, '_'},
            {DataBaseProvider.PostgreSql, '_'},
            {DataBaseProvider.SQLite, '_'},
            {DataBaseProvider.SqlServer, '_'}
        };

        private readonly DataBaseProvider _provider;
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly char _delimiter;

        private DbColumnBuilder(DataBaseProvider provider)
        {
            _provider = provider;
            _delimiter = ResolveDelimiter(provider);
        }

        public static DbColumnBuilder CreateWithNormalizedValue(string value, DataBaseProvider provider)
        {
            var result = new DbColumnBuilder(provider);
            return result.AddNormalized(value);
        }

        public static DbColumnBuilder CreateWithPristineValue(string value, DataBaseProvider provider)
        {
            var result = new DbColumnBuilder(provider);
            return result.AddPristine(value);
        }

        public DbColumnBuilder AddNormalized(string value)
        {
            if (string.IsNullOrEmpty(value))
                return this;

            _sb.Append(Normalize(value));
            AddDelimiter();
            return this;
        }

        public DbColumnBuilder AddPristine(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return this;

            _sb.Append(value);
            AddDelimiter();
            return this;
        }

        public string Build()
        {
            if (_sb.Length == 0)
                throw new InvalidOperationException("Building an empty column name is not allowed");

            if (_delimiter != default && _sb.Length > 0)
                return _sb.ToString(0, _sb.Length - 1);
            return _sb.ToString();
        }

        private static char ResolveDelimiter(DataBaseProvider provider)
        {
            return DelimiterMap.ContainsKey(provider) ? DelimiterMap[provider] : default;
        }

        private void AddDelimiter()
        {
            if (_delimiter != default)
                _sb.Append(_delimiter);
        }

        private string Normalize(string value)
        {
            switch (_provider)
            {
                case DataBaseProvider.SqlServer:
                case DataBaseProvider.Oracle:
                    return value.ToUpperSnakeCase();

                case DataBaseProvider.PostgreSql:
                case DataBaseProvider.SQLite:
                    return value.ToLowerSnakeCase();
                default:
                    return value;
            }
        }
    }
}