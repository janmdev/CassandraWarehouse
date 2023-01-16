using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraWarehouse.Models
{
    public class Receiving
    {
        public Receiving(Guid? id, string? number, DateTimeOffset date, string? client, string? positions)
        {
            Id = id;
            Number = number;
            Date = date;
            Client = client;
            Positions = positions;
        }

        public Guid? Id { get; set; }
        public string? Number { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Client { get; set; }
        public string? Positions { get; set; }
    }
}
