using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class KestrelOptions
    {
        public bool EnableKestrel { get; set; }

        public bool IsPortValid => ListeningPort > 1023 && ListeningPort <= 655535;

        public List<string> ListeningIps { get; set; } = new List<string>();
        public bool EnableLocalHostListening { get; set; }
        public bool EnableAnyIpListening { get; set; }
        public int ListeningPort { get; set; }
        public bool EnableHttps { get; set; }

        public bool UseForwarders { get; set; }
        public List<string> ValidForwarders { get; set; } = new List<string>();
    }
}