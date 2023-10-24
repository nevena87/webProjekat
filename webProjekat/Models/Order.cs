using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webProjekat.Models
{
    public enum Status
    {
        Active, Executed, Cancelled
    }

    public class Order
    {
        string product;
        int quantity;
        string customer;
        DateTime orderDate;
        Status status;
        bool deleted;

        public string Product { get => product; set => product = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public string Customer { get => customer; set => customer = value; }
        public DateTime OrderDate { get => orderDate; set => orderDate = value; }
        public Status Status { get => status; set => status = value; }
        public bool Deleted { get => deleted; set => deleted = value; }

        public Order(string product, int quantity, string customer, DateTime orderDate, Status status)
        {
            Product = product;
            Quantity = quantity;
            Customer = customer;
            OrderDate = orderDate;
            Status = status;
            Deleted = false;
        }

        public Order()
        {
        }
    }
}