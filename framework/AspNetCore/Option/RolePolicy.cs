using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class RolePolicy
    {
        public string Name { get; set; }
        public List<string> Groups { get; set; } = new List<string>();
    }
}