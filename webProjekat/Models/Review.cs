using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webProjekat.Models
{
    public class Review
    {
        string product;
        string username;
        string title;
        string content;
        string image;
        bool accepted;
        bool declined;
        bool deleted;

        public string Product { get => product; set => product = value; }
        public string Username { get => username; set => username = value; }
        public string Title { get => title; set => title = value; }
        public string Content { get => content; set => content = value; }
        public bool Accepted { get => accepted; set => accepted = value; }
        public bool Declined { get => declined; set => declined = value; }
        public bool Deleted { get => deleted; set => deleted = value; }
        public string Image { get => image; set => image = value; }

        public Review(string product, string username, string title, string content, string image, bool accepted, bool declined)
        {
            Product = product;
            Username = username;
            Title = title;
            Content = content;
            Image = image;
            Accepted = accepted;
            Declined = declined;
        }

        public Review()
        {
        }
    }
}