using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraWarehouse
{
    public static class Extensions
    {
        public static LocalDate ToLocalDate(this DateTime dateTime)
        {
            return new LocalDate(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
