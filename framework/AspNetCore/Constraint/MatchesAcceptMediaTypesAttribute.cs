using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Net.Http.Headers;
using System.Linq;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Constraint
{
    public class MatchesAcceptMediaTypesAttribute : MatchesBaseAttribute
    {
        private static readonly string AcceptHeader = "Accept";

        public MatchesAcceptMediaTypesAttribute(params string[] parameters) : base(parameters)
        {
        }

        public override bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

            if (!requestHeaders.ContainsKey(AcceptHeader))
                return false;

            if (!MediaTypeHeaderValue.TryParseList(requestHeaders[AcceptHeader].ToString().Split(','), out var requestedTypes))
                return false;

            return requestedTypes.Any(requestedType => SupportedMediaTypes.Contains(requestedType.MediaType));
        }

        public override int Order => 0;
    }
}