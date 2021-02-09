using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extension;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Models
{
    public sealed class LexiconDictionary
    {
        private const int LexiconInitialBucketSize = 25;

        //
        private readonly string _defaultLanguage;

        private readonly int _defaultLanguageId;

        private readonly string[][,] _dictionnaryMatrix;                                  // [lexiconId] [langId , sorted values of default language]>

        // <entityName, <from_property_name, Lexicon object >>
        private readonly Dictionary<string, Dictionary<string, Lexicon[]>> _entityLexicons;

        private readonly Dictionary<string, int> _languages;                              // <language iso 2 code, langId>
        private readonly int _lexiconCount;

        public LexiconDictionary(IEnumerable<Language> languages, IEnumerable<Lexicon> lexicons, IEnumerable<LexiconItem> lexiconItems)
        {
            #region sort items

            lexiconItems = lexiconItems.OrderBy(s => s.Value);

            #endregion sort items

            #region load _languages

            // get distinct languages
            var distinctLanguages = new HashSet<long>();
            var tempLanguage = new Dictionary<long, int>(languages.Count() * 4);
            foreach (var lexiconItem in lexiconItems)
                if (lexiconItem.LanguageId != null && !distinctLanguages.Contains((long)lexiconItem.LanguageId))
                    distinctLanguages.Add((long)lexiconItem.LanguageId);

            // load _languages dictionary
            _languages = new Dictionary<string, int>((distinctLanguages.Count + 1) * 4);
            foreach (var language in languages)
            {
                if (string.IsNullOrEmpty(language.IsoCode)) continue;
                if (language.Default) _defaultLanguage = language.IsoCode.ToUpper();
                if (!_languages.ContainsKey(language.IsoCode.ToUpper()) && (distinctLanguages.Contains(language.Id) || language.Default))
                    _languages.Add(language.IsoCode.ToUpper(), _languages.Count);
                if (!tempLanguage.ContainsKey(language.Id) && _languages.ContainsKey(language.IsoCode.ToUpper()))
                    tempLanguage.Add(language.Id, _languages[language.IsoCode.ToUpper()]);
            }

            // define default language
            if (string.IsNullOrEmpty(_defaultLanguage) && _languages.Count > 0)
                _defaultLanguage = _languages.OrderBy(kvp => kvp.Key).First().Key;
            _defaultLanguageId = _languages[_defaultLanguage];

            #endregion load _languages

            #region load _entityLexicons

            // get distinct entities
            _entityLexicons = new Dictionary<string, Dictionary<string, Lexicon[]>>(lexicons.Count() * 4);

            int index = 0;
            var tempLexicon = new Dictionary<long, Lexicon>(lexicons.Count() * 4); // mapper: <lexiconId, LexiconId>
            foreach (var lex in lexicons)
            {
                if (!_entityLexicons.ContainsKey(lex.EntityName)) _entityLexicons.Add(lex.EntityName, new Dictionary<string, Lexicon[]>(LexiconInitialBucketSize));
                var currentDico = _entityLexicons[lex.EntityName];
                var fromProp = string.IsNullOrEmpty(lex.FromProperty) ? string.Empty : lex.FromProperty;
                //
                if (!currentDico.ContainsKey(fromProp))
                {
                    lex.LocalLexiconId = index;
                    var targetArray = new Lexicon[1];
                    targetArray[0] = lex;
                    currentDico.Add(fromProp, targetArray);
                    tempLexicon.Add(lex.Id, lex);
                    ++index;
                }
                else
                {
                    lex.LocalLexiconId = index;
                    var arr = currentDico[fromProp];
                    Array.Resize(ref arr, arr.Length + 1);
                    arr[arr.Length - 1] = lex;
                    currentDico[fromProp] = arr;
                    tempLexicon.Add(lex.Id, lex);
                    ++index;
                }
            }
            _lexiconCount = index;

            #endregion load _entityLexicons

            #region load _dictionnaryMatrix

            // get distinct LexiconItem.value
            var distinctValues = new Dictionary<string, string>(lexiconItems.Count() * 2); // copy all distinct values references
            foreach (var lexiconItem in lexiconItems)
                if (!distinctValues.ContainsKey(lexiconItem.Value))
                    distinctValues.Add(lexiconItem.Value, lexiconItem.Value);

            // calculate matrices sizes (PASSING #1) - O(n)
            _dictionnaryMatrix = new string[_lexiconCount][,];
            foreach (var lexiconItem in lexiconItems)
            {
                var currentLexicon = lexiconItem.LexiconId == null ? null : tempLexicon[(long)lexiconItem.LexiconId];
                // count sources
                if (currentLexicon != null) ++currentLexicon.ItemCount;
            }

            // allow matrices structure
            foreach (var keyValue in tempLexicon)
                if (keyValue.Value.ItemCount > 0)
                    _dictionnaryMatrix[keyValue.Value.LocalLexiconId] =
                        new string[_languages.Count, keyValue.Value.ItemCount];

            var lexiconItemIndex = new int[_lexiconCount];
            var sourceDico = new Dictionary<long, Tuple<int, int>>(lexiconItems.Count()); // <LexiconItemId (from DB), Tuple<local LexiconItem, LexiconId>>

            // load default language (PASSING #2) - O(n)
            foreach (var lexiconItem in lexiconItems)
            {
                var currentLexicon = lexiconItem.LexiconId == null ? null : tempLexicon[(long)lexiconItem.LexiconId];

                // default language
                if (currentLexicon != null)
                {
                    var currentLexiconId = currentLexicon.LocalLexiconId;
                    _dictionnaryMatrix[currentLexiconId][_defaultLanguageId, lexiconItemIndex[currentLexiconId]] = distinctValues[lexiconItem.Value];
                    sourceDico.Add(lexiconItem.Id, new Tuple<int, int>(lexiconItemIndex[currentLexiconId], currentLexicon.LocalLexiconId));
                    ++lexiconItemIndex[currentLexiconId];
                }
            }

            // load default language (PASSING #3) - O(n)
            foreach (var lexiconItem in lexiconItems)
                // target language
                if (lexiconItem.LexiconId == null && lexiconItem.TargetItemId != null && lexiconItem.LanguageId != null)
                {
                    var LexItemId = sourceDico[(long)lexiconItem.TargetItemId].Item1;
                    var LexId = sourceDico[(long)lexiconItem.TargetItemId].Item2;
                    var langId = tempLanguage[(long)lexiconItem.LanguageId];
                    _dictionnaryMatrix[LexId][langId, LexItemId] = distinctValues[lexiconItem.Value];
                }

            #endregion load _dictionnaryMatrix
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMultiLingualEntity(Type type)
        {
            return type != null && _entityLexicons.ContainsKey(type.Name);
        }

        public T Translate<T>(T entity, string language) where T : class, IEntityBase
        {
            if (entity == null || string.IsNullOrEmpty(language)) return entity;
            var langId = _languages.ContainsKey(language.ToUpper().Trim()) ? _languages[language.ToUpper().Trim()] : _defaultLanguageId;
            if (langId == _defaultLanguageId) return entity;
            var entityName = typeof(T).Name;
            var entityLexicons = _entityLexicons.ContainsKey(entityName) ? _entityLexicons[entityName] : null;
            if (entityLexicons == null) return entity;
            return Translate(entity, langId, entityLexicons);
        }

        public IEnumerable<T> Translate<T>(IEnumerable<T> entities, string language) where T : class, IEntityBase
        {
            if (entities == null || string.IsNullOrEmpty(language) || !entities.Any()) return entities;
            var langId = _languages.ContainsKey(language.ToUpper().Trim()) ? _languages[language.ToUpper().Trim()] : _defaultLanguageId;
            if (langId == _defaultLanguageId) return entities;
            var entityName = typeof(T).Name;
            var entityLexicons = _entityLexicons.ContainsKey(entityName) ? _entityLexicons[entityName] : null;
            if (entityLexicons == null) return entities;
            var result = new T[entities.Count()];
            var index = 0;
            foreach (var entity in entities) result[index++] = Translate(entity, langId, entityLexicons);
            return result;
        }

        private T Translate<T>(T entity, int langId, Dictionary<string, Lexicon[]> matrices) where T : class, IEntityBase
        {
            var result = entity;
            foreach (var property in result.GetType().GetProperties())
                if (matrices.ContainsKey(property.Name))
                {
                    var mat = matrices[property.Name];
                    if (mat == null) continue;
                    var sourceValue = property.GetValue(result)?.ToString();
                    for (var i = 0; i < mat.Length; ++i)
                    {
                        var lexicon = mat[i];
                        var targetValue = FindValue(langId, lexicon.LocalLexiconId, sourceValue);
                        if (!string.IsNullOrEmpty(targetValue))
                        {
                            if (string.CompareOrdinal(lexicon.FromProperty, lexicon.ToProperty) == 0)
                            {
                                property.SetValue(result, targetValue);
                            }
                            else
                            {
                                var targetProperty = typeof(T).GetProperty(lexicon.ToProperty, BindingFlags.Public | BindingFlags.Instance);
                                targetProperty.SetValue(result, targetValue);
                            }
                        }
                    }
                }

            return result;
        }

        public bool IsTranslationExists<T>(T value = null) where T : class
        {
            return _entityLexicons.ContainsKey(GetKeyFromType<T>());
        }

        public string GetKeyFromType<T>() where T : class
        {
            return $"[{typeof(T).Name}]";
        }

        public string Translate<T>(T value, Expression<Func<T, string>> stringSelector, string language) where T : class
        {
            var languageId = _languages.GetValueOrDefault(language.TrimToUpper(), -1);
            if (languageId == -1)
                return null;

            // <entityName, <from_property_name, Lexicon object >>
            //private readonly Dictionary<string, Dictionary<string, Lexicon[]>> _entityLexicons;

            var partialLexicons = _entityLexicons.GetValueOrDefault(GetKeyFromType<T>(), null);

            if (partialLexicons == null)
                return null;

            var lexicons = partialLexicons.GetValueOrDefault(stringSelector.GetPropertyName(), null);
            if (!(lexicons?.Any() ?? false))
                return null;

            var lexiconId = lexicons.First().LocalLexiconId;

            return FindValue(languageId, lexiconId, stringSelector.Compile()(value));
        }

        /// <summary>
        /// Find into matrix current value for current LangId
        /// </summary>
        private string FindValue(int langId, int lexiconId, string value)
        {
            if (value == null || _dictionnaryMatrix[lexiconId] == null) return null;

            int indexerLeft = 0, indexerRigth = _dictionnaryMatrix[lexiconId].GetLength(1) - 1;
            while (indexerLeft <= indexerRigth)
            {
                var indexerMiddle = indexerLeft + indexerRigth;
                indexerMiddle >>= 1;   // indexerMiddle <-- indexerMiddle /2
                var indexerCompare = string.CompareOrdinal(value, _dictionnaryMatrix[lexiconId][_defaultLanguageId, indexerMiddle]);
                if (indexerCompare == 0) return _dictionnaryMatrix[lexiconId][langId, indexerMiddle];
                if (indexerCompare > 0) indexerLeft = indexerMiddle + 1;
                else indexerRigth = indexerMiddle - 1;
            }
            return null;
        }
    }
}