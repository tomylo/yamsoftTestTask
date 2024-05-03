using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Configurations
{

    /// <summary>
    /// Classes for configuration.
    /// </summary>
    public class YSConfiguration
    {
        public DbSettings DbSettings { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public LogSettings LogSettings { get; set; }
        public GenericSettings GenericSettings { get; set; }
    }

    public class GenericSettings
    {
        public bool CheckForSeedData { get; set; }
        public string ApiServer { get; set; }
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

    public class LogSettings
    {
        public string Folder { get; set; }
        public string TableName { get; set; }
        public RollingInterval RollingInterval { get; set; }
        public LogEventLevel LogEventLevel { get; set; }
    }
}
