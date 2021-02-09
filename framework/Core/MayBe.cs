using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Infrabel.ICT.Framework.Extension;

namespace Infrabel.ICT.Framework
{
    public static class Maybe
    {
        public static Maybe<T> WithValue<T>(T value)
        {
            return Maybe<T>.WithValue(value);
        }

        public static Maybe<T> WithNoValue<T>()
        {
            return Maybe<T>.WithNoValue();
        }
    }

    public class Maybe<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();

        private Maybe()
        {
        }

        private Maybe(T item)
        {
            if (item != null)
                _list.Add(item);
        }

        public bool HasValue => _list.Any();

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Maybe has no value");
                return _list.Single();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator Maybe<T>(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> WithValue(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> WithNoValue()
        {
            return new Maybe<T>();
        }

        public Maybe<TProperty> Get<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            if (!HasValue)
                return Maybe<TProperty>.WithNoValue();

            var memberExpression = selector.Body as MemberExpression;
            var propertyInfo = memberExpression?.Member as PropertyInfo;

            return propertyInfo != null ? Maybe<TProperty>.WithValue((TProperty)propertyInfo.GetValue(Value, null)) : Maybe<TProperty>.WithNoValue();
        }

        public Maybe<TProperty> GetMaybe<TProperty>(Expression<Func<T, Maybe<TProperty>>> selector)
        {
            if (!HasValue)
                return Maybe<TProperty>.WithNoValue();

            var propertyInfo = selector.GetPropertyInfo();
            var result = (Maybe<TProperty>)propertyInfo.GetValue(Value, null);

            return result.HasValue ? Maybe<TProperty>.WithValue(result.Value) : Maybe<TProperty>.WithNoValue();
        }

        public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
        {
            return !HasValue ? Maybe<TResult>.WithNoValue() : map(Value);
        }

        public T Reduce(Func<T> noValueFactory)
        {
            return HasValue ? Value : noValueFactory();
        }

        public T Reduce(T noValue)
        {
            return HasValue ? Value : noValue;
        }
    }
}