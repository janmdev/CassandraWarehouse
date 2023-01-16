using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraWarehouse.Models
{
    public class Release
    {
        public Release(Guid? id, string? number, LocalDate date, string? client, string? positions)
        {
            Id = id;
            Number = number;
            Date = date;
            Client = client;
            Positions = positions;
        }

        public Guid? Id { get; set; }
        public string? Number { get; set; }
        public LocalDate Date { get; set; }
        public string? Client { get; set; }
        public string? Positions { get; set; }
    }
}
