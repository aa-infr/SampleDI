using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Net.Http.Headers;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Constraint
{
    public class MatchesContentTypeMediaTypeAttribute : MatchesBaseAttribute
    {
        private static readonly string ContentTypeHeader = "Content-Type";

        public MatchesContentTypeMediaTypeAttribute(params string[] parameters) : base(parameters)
        {
        }

        public override bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

            if (!requestHeaders.ContainsKey(ContentTypeHeader))
                return false;

            if (!MediaTypeHeaderValue.TryParse(requestHeaders[ContentTypeHeader].ToString(), out var contentType))
                return false;

            return SupportedMediaTypes.Contains(contentType.MediaType);
        }

        public override int Order => 0;
    }
}