using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class LocalizationOptions
    {
        public List<string> SupportedCultures { get; set; } = new List<string>();
    }
}