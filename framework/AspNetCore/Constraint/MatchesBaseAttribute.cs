using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Constraint
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class MatchesBaseAttribute : Attribute, IActionConstraint
    {
        protected readonly ISet<StringSegment> SupportedMediaTypes;

        protected MatchesBaseAttribute(params string[] parameters)
        {
            SupportedMediaTypes = Map(parameters);
        }

        public static ISet<StringSegment> Map(params string[] parameters)
        {
            var result = new HashSet<StringSegment>();

            foreach (var parameter in parameters)
            {
                if (!MediaTypeHeaderValue.TryParse(parameter, out var mediaTypeHeader))
                    throw new FormatException($"The media type format is not valid {parameter}");

                if (result.Contains(parameter))
                    throw new InvalidOperationException($"{parameter} has already been defined");

                result.Add(mediaTypeHeader.MediaType);
            }

            return result;
        }

        public abstract bool Accept(ActionConstraintContext context);

        public abstract int Order { get; }
    }
}