using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraWarehouse.Models
{
    public class Releases
    {
        public string? Number { get; set; }
        public DateTime? Date { get; set; }
        public string? Client { get; set; }
        public string? Positions { get; set; }
    }
}
