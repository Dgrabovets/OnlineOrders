using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrders.MVVM.Model
{
    public class Product
    {
        public int ID { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }

    public class OrderProduct : Product
    {
        public int OrderID { get; set; }

        public bool IsCreated { get; set; } = false;
    }
}
