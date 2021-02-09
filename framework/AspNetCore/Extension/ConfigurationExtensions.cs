using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Extension
{
    public static class ConfigurationExtensions
    {
        public static readonly string OptionsValidationErrorCode = "OptionsValidation";

        public static TOptions GetOptions<TOptions>(this IConfiguration configuration) where TOptions : class, new()
        {
            var options = new TOptions();

            configuration.GetSection(typeof(TOptions).Name)
                .Bind(options);

            var context = new ValidationContext(options, null);

            ICollection<ValidationResult> results = new List<ValidationResult>();


            return options;
        }
    }
}