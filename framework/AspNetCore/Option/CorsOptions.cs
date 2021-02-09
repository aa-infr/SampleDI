using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class CorsOptions
    {
        public bool EnableCors { get; set; }
        public List<string> AllowedOrigins { get; set; } = new List<string>();
    }
}