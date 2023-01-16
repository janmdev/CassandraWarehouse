using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraWarehouse.Models
{
    public class Client
    {
        public Client(Guid? id, string? name)
        {
            Id = id;
            Name = name;
        }

        public Guid? Id { get; set; }

        public string? Name { get; set; }
    }
}
