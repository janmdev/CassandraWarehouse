using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraWarehouse.Models
{
    public class Stock
    {
        public Stock()
        {
        }

        public Stock(Guid? recv, Guid? ware, long? quantity)
        {
            Receiving = recv;
            Ware = ware;
            Quantity = quantity;
        }

        public Stock(Guid? recv, Guid? ware, long? quantity, double? price)
        {
            Receiving = recv;
            Ware = ware;
            Quantity = quantity;
            Price = price;
        }

        public Guid? Receiving { get; set; }
        public Guid? Ware { get; set; }
        public long? Quantity { get; set; }
        public double? Price { get; set; }
    }
}
