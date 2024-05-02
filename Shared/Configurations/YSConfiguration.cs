using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Configurations
{
    public class YSConfiguration
    {
        public DbSettings DbSettings { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }
    public class DbSettings
    {
        public string ConnectionString { get; set; }
    }

    public class JwtSettings
    {
        public string Audience { get; set; }
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
        public string Issuer { get; set; }
    }
}
