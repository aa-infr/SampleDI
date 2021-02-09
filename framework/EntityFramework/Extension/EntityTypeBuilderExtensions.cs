using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Extended.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Extension
{
    public static class EntityTypeBuilderExtensions
    {
        private static readonly string ColumnFrench = "Fr";
        private static readonly string ColumnDutch = "Nl";
        private static readonly string ColumnEnglish = "En";

        private const string SearchablePrefix = "S";
        private const int BucketCapacity = 512;
        private const int LexiconBucketCapacity = 20;

        // config cache !! - <entity_name, <property_name, column_name> >
        private static readonly Dictionary<string, SortedDictionary<string, SearchableInfo>> _searchableFields =
                                        new Dictionary<string, SortedDictionary<string, SearchableInfo>>(BucketCapacity);

        private static readonly Dictionary<string, HashSet<string>> _multiLingualFields = new Dictionary<string, HashSet<string>>(BucketCapacity);

        // config lexicon cache
        private static readonly Dictionary<Type, SortedDictionary<string, string>> _lexiconFields = new Dictionary<Type, SortedDictionary<string, string>>(LexiconBucketCapacity)
        {
            [typeof(ILanguage)] = new SortedDictionary<string, string>(),
            [typeof(ILexicon)] = new SortedDictionary<string, string>(),
            [typeof(ILexiconItem)] = new SortedDictionary<string, string>()
        };

        private static readonly object _syncRoot = new object();  // is ILexiconItem entity defined ?

        // lexicon entities defined ?
        private static bool EntityILanguage = false;     // is ILanguage entity defined ?

        private static bool EntityILexicon = false;      // is ILexicon entity defined ?
        private static bool EntityILexiconItem = false;  // is ILexiconItem entity defined ?
        private static readonly Type MultilingualType = typeof(MultilingualString);

        private static PropertyBuilder RefineProperty(this PropertyBuilder builder, bool concurrencyToken, bool isRequired, int? maxLength)
        {
            builder = builder.IsConcurrencyToken(concurrencyToken);
            if (maxLength.HasValue)
                builder = builder.HasMaxLength(maxLength.Value);
            if (isRequired)
                builder = builder.IsRequired(isRequired);

            return builder;
        }

        /// <summary>
        /// Add property (threadsafe!)
        /// </summary>
        public static void AddProperty<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> propertyExpression,
            string columnName, bool required = false, bool concurrencyToken = false, int? maxLength = null)
            where TEntity : class, IEntityBase, new()
        {
            var member = propertyExpression.Body as MemberExpression;
            var propInfo = member?.Member as PropertyInfo;

            var provider = EntityBaseConfiguration<TEntity>.GetProvider();

            var propertyName = propInfo?.Name;

            var columnBuilder = string.IsNullOrWhiteSpace(columnName)
                                    ? DbColumnBuilder.CreateWithNormalizedValue(propertyName, provider)
                                    : DbColumnBuilder.CreateWithPristineValue(columnName, provider);

            lock (_syncRoot)
            {
                // add column
                if (propInfo?.PropertyType == MultilingualType)
                {
                    builder.OwnsOne(typeof(MultilingualString), propInfo.Name, l =>
                    {
                        var baseColumnName = columnBuilder.Build();

                        RefineProperty(l.Property(nameof(MultilingualString.Dutch))
                            .HasColumnName(DbColumnBuilder.CreateWithPristineValue(baseColumnName, provider).AddNormalized(ColumnDutch).Build()),
                            concurrencyToken,
                            required,
                            maxLength);

                        RefineProperty(l.Property(nameof(MultilingualString.French))
                            .HasColumnName(DbColumnBuilder.CreateWithPristineValue(baseColumnName, provider).AddNormalized(ColumnFrench).Build()),
                            concurrencyToken,
                            required,
                            maxLength);
                        RefineProperty(l.Property(nameof(MultilingualString.English))
                            .HasColumnName(DbColumnBuilder.CreateWithPristineValue(baseColumnName, provider).AddNormalized(ColumnEnglish).Build()),
                            concurrencyToken,
                            required,
                            maxLength);
                    });
                }
                else
                {
                    var propertyBuilder = RefineProperty(builder.Property(propertyExpression), concurrencyToken, required, maxLength);
                    var col = columnBuilder.Build();
                    propertyBuilder.HasColumnName(col);

                    var searchableType = GetSearchableType(propInfo);

                    // add shadow property for searchable fields
                    if (propInfo.PropertyType == typeof(string) && searchableType != null)
                    {
                        // eg. builder.Property<string>("S_LAST_NAME");
                        var entityName = typeof(TEntity).FullName;
                        var searchableField = DbColumnBuilder.CreateWithNormalizedValue(SearchablePrefix, provider)
                                                             .AddPristine(col)
                                                             .Build();

                        builder.Property<string>(searchableField);
                        if (!_searchableFields.ContainsKey(entityName)) _searchableFields.Add(entityName, new SortedDictionary<string, SearchableInfo>());
                        if (!_searchableFields[entityName].ContainsKey(propertyName))
                            _searchableFields[entityName].Add(propertyName, new SearchableInfo((SearchableType)searchableType, searchableField));
                    }

                    if (IsMultiLingual(propInfo))
                    {
                        var entityName = typeof(TEntity).FullName;

                        if (!_multiLingualFields.ContainsKey(entityName)) _multiLingualFields.Add(entityName, new HashSet<string>());
                        if (!_multiLingualFields[entityName].Contains(propertyName)) _multiLingualFields[entityName].Add(propertyName);
                    }

                    #region lexicon info

                    //*** Language
                    if (!EntityILanguage && typeof(ILanguage).IsAssignableFrom(typeof(TEntity)))
                    {
                        var properties = typeof(ILanguage).GetProperties();
                        // enter only info about ILanguage
                        if (properties.Exist(propInfo.Name))
                        {
                            if (!_lexiconFields[typeof(ILanguage)].ContainsKey(propInfo.Name)) _lexiconFields[typeof(ILanguage)].Add(propInfo.Name, col);
                            else _lexiconFields[typeof(ILanguage)][propInfo.Name] = col;
                            // Are properties completly defined ?
                            if (properties.Length == _lexiconFields[typeof(ILanguage)].Count) EntityILanguage = true;
                        }
                    }
                    //*** Lexicon
                    if (!EntityILexicon && typeof(ILexicon).IsAssignableFrom(typeof(TEntity)))
                    {
                        var properties = typeof(ILexicon).GetProperties();
                        // enter only info about ILexicon
                        if (properties.Exist(propInfo.Name))
                        {
                            if (!_lexiconFields[typeof(ILexicon)].ContainsKey(propInfo.Name)) _lexiconFields[typeof(ILexicon)].Add(propInfo.Name, col);
                            else _lexiconFields[typeof(ILexicon)][propInfo.Name] = col;
                            // Are properties completly defined ?
                            if (typeof(ILexicon).GetProperties().Length == _lexiconFields[typeof(ILexicon)].Count) EntityILexicon = true;
                        }
                    }
                    //*** LexiconItem
                    if (!EntityILexiconItem && typeof(ILexiconItem).IsAssignableFrom(typeof(TEntity)))
                    {
                        var properties = typeof(ILexiconItem).GetProperties();
                        // enter only info about ILexiconItem
                        if (properties.Exist(propInfo.Name))
                        {
                            if (!_lexiconFields[typeof(ILexiconItem)].ContainsKey(propInfo.Name)) _lexiconFields[typeof(ILexiconItem)].Add(propInfo.Name, col);
                            else _lexiconFields[typeof(ILexiconItem)][propInfo.Name] = col;
                            // Are properties completly defined ?
                            if (typeof(ILexiconItem).GetProperties().Length == _lexiconFields[typeof(ILexiconItem)].Count) EntityILexiconItem = true;
                        }
                    }

                    #endregion lexicon info
                }
            }
        }

        public static void AddProperty<TEntity, TProperty>(
            this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, TProperty>> propertyExpression, bool required = false, bool concurrencyToken = false, int? maxLength = null)
            where TEntity : class, IEntityBase, new()
         => AddProperty(builder, propertyExpression, null, required, concurrencyToken, maxLength);

        internal static SearchableInfo GetShadowField(Type type, PropertyInfo property)
        {
            if (type == null || property == null) return null;
            if (_searchableFields.ContainsKey(type.FullName))
            {
                var propDico = _searchableFields[type.FullName];
                if (propDico.ContainsKey(property.Name)) return propDico[property.Name];
            }
            return null;
        }

        internal static SearchableInfo GetShadowField(Type type, PropertyEntry property)
        {
            if (type == null || property == null) return null;
            if (_searchableFields.ContainsKey(type.FullName))
            {
                var propDico = _searchableFields[type.FullName];
                if (propDico.ContainsKey(property.Metadata.Name)) return propDico[property.Metadata.Name];
            }
            return null;
        }

        internal static SearchableInfo GetShadowField(Type type, string propertyName)
        {
            if (type == null || propertyName == null) return null;
            if (_searchableFields.ContainsKey(type.FullName))
            {
                var propDico = _searchableFields[type.FullName];
                if (propDico.ContainsKey(propertyName)) return propDico[propertyName];
            }
            return null;
        }

        internal static bool IsLexiconEntityAvailable => EntityILanguage && EntityILexiconItem && EntityILexiconItem;

        internal static bool HasShadowField(Type type)
        {
            if (type == null) return false;
            return _searchableFields.ContainsKey(type.FullName);
        }

        internal static string GetSelect(Type type)
        {
            string[] result = null;
            if (typeof(ILanguage).IsAssignableFrom(type)) result = _lexiconFields[typeof(ILanguage)].Select(z => z.Value).ToArray();
            if (typeof(ILexicon).IsAssignableFrom(type)) result = _lexiconFields[typeof(ILexicon)].Select(z => z.Value).ToArray();
            if (typeof(ILexiconItem).IsAssignableFrom(type)) result = _lexiconFields[typeof(ILexiconItem)].Select(z => z.Value).ToArray();
            return result == null ? string.Empty : string.Join(",", result);
        }

        internal static string[] GetProperties(Type type)
        {
            string[] result = null;
            if (typeof(ILanguage).IsAssignableFrom(type)) result = _lexiconFields[typeof(ILanguage)].Select(z => z.Key).ToArray();
            if (typeof(ILexicon).IsAssignableFrom(type)) result = _lexiconFields[typeof(ILexicon)].Select(z => z.Key).ToArray();
            if (typeof(ILexiconItem).IsAssignableFrom(type)) result = _lexiconFields[typeof(ILexiconItem)].Select(z => z.Key).ToArray();
            return result;
        }

        private static SearchableType? GetSearchableType(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return null;
            var attrs = propertyInfo.GetCustomAttributes(true);
            foreach (var attr in attrs) if (attr is SearchableAttribute attribute) return attribute.Type;
            return null;
        }

        private static bool IsMultiLingual(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return false;
            var attrs = propertyInfo.GetCustomAttributes(true);
            foreach (var attr in attrs) if (attr is MultiLingualAttribute) return true;
            return false;
        }
    }
}