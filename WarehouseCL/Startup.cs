using Cassandra;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseCL
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            CassSettings = config.GetSection("CassandraSettings").Get<CassandraSettings>();

        }

        public CassandraSettings CassSettings { get; private set; }
    }
}
