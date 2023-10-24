using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webProjekat.Models
{
    public enum Gender
    {
        Male, Female
    }

    public enum Role
    {
        Customer, Seller, Administrator
    }

    public class User
    {
        string username;
        string password;
        string firstName;
        string lastName;
        Gender gender;
        string email;
        DateTime birthday;
        Role role;
        List<Order> orders;
        List<Product> favoriteProducts;
        List<string> releasedProducts;
        bool deleted;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public Gender Gender { get => gender; set => gender = value; }
        public string Email { get => email; set => email = value; }
        public DateTime Birthday { get => birthday; set => birthday = value; }
        public Role Role { get => role; set => role = value; }
        public List<Order> Orders { get => orders; set => orders = value; }
        public List<Product> FavoriteProducts { get => favoriteProducts; set => favoriteProducts = value; }
        public List<string> ReleasedProducts { get => releasedProducts; set => releasedProducts = value; }
        public bool Deleted { get => deleted; set => deleted = value; }

        public User(string username, string password, string firstName, string lastName, Gender gender, string email, DateTime birthday, Role role, bool deleted)
        {
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            Email = email;
            Birthday = birthday;
            Role = role;
            Deleted = false;
            Orders = new List<Order>();
            FavoriteProducts = new List<Product>();
            ReleasedProducts = new List<string>();
        }

        public User()
        {
        }
    }
}