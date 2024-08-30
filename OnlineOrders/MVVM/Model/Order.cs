using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrders.MVVM.Model
{
    public class Order
    {
        public int ID { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal Total { get; set; }

        public int ClientID { get; set; }

        public string ClientFullName { get; set; }

        public string OrderStatus { get; set; }

        public string Comment { get; set; }

        public ObservableCollection<OrderProduct> Products { get; set; } = new ObservableCollection<OrderProduct>();

        public bool IsCreated { get; set; } = false;
    }
}
