using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    /// <summary>
    ///
    /// </summary>
    internal class DbLexiconRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbConnection _connection;
        private readonly EntityBaseConfiguration<T> _configuration;

        public DbLexiconRepository(DbConnection connection, EntityBaseConfiguration<T> configuration)
        {
            _connection = connection;
            _configuration = configuration;
        }

        public IList<T> GetAll()
        {
            var result = new List<T>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = _configuration.GetSql();
                //var propertyInfoList = GetPropertiesInfo();
                var propertyInfoList = _configuration.GetEntityProperties();

                if (propertyInfoList.Length <= 0) return null;

                PropertyInfo propInfoId = typeof(T).GetProperty(nameof(EntityBase.Id));
                PropertyInfo propInfo;
                var readerIndex = 0;
                var tempValue = 0L;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var instance = (T)Activator.CreateInstance(typeof(T));
                        // id
                        readerIndex = 0;
                        propInfoId.SetValue(instance, reader.GetInt64(readerIndex), null);
                        // other fields
                        foreach (var propName in propertyInfoList)
                        {
                            ++readerIndex;
                            propInfo = typeof(T).GetProperty(propName);
                            var value = reader.GetValue(readerIndex).ToString();
                            if (value != null && long.TryParse(value, out tempValue))
                            {
                                if (propInfo.PropertyType == typeof(long) || propInfo.PropertyType == typeof(long?))
                                    propInfo.SetValue(instance, long.Parse(value));
                                if (propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(int?))
                                    propInfo.SetValue(instance, int.Parse(value));
                                if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
                                    propInfo.SetValue(instance, "1".Equals(value));
                            }
                            if (propInfo.PropertyType == typeof(string)) propInfo.SetValue(instance, value);
                        }
                        result.Add(instance);
                    }
                }
            }
            return result;
        }
    }
}