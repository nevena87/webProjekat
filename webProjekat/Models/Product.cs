using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webProjekat.Models
{
    public enum StatusAUn
    {
        Available, Unavailable
    }

    public class Product
    {
        string name;
        double price;
        int quantity;
        string description;
        string image;
        DateTime productPlacementDate = DateTime.Now;
        string city;
        List<Review> reviews;
        StatusAUn status;
        bool deleted;

        public string Name { get => name; set => name = value; }
        public double Price { get => price; set => price = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public string Description { get => description; set => description = value; }
        public DateTime ProductPlacementDate { get => productPlacementDate; set => productPlacementDate = value; }
        public string City { get => city; set => city = value; }
        public List<Review> Reviews { get => reviews; set => reviews = value; }
        public bool Deleted { get => deleted; set => deleted = value; }
        public StatusAUn Status { get => status; set => status = value; }
        public string Image { get => image; set => image = value; }

        public Product()
        {
        }

        public Product(string name, double price, int quantity, string description, string image, DateTime productPlacementDate, string city, StatusAUn status, bool deleted)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            Description = description;
            Image = image;
            ProductPlacementDate = productPlacementDate;
            City = city;
            Status = status;
            this.Deleted = false;
            Reviews = new List<Review>();
        }
    }
}