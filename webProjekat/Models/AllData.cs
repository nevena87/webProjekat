using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace webProjekat.Models
{
    public class AllData
    {
        static string productsLocation = "C:/Users/Nevena/Downloads/webProjekat/webProjekat/products.xml";
        static string favoriteProductsLocation = "C:/Users/Nevena/Downloads/webProjekat/webProjekat/favoriteProducts.xml";
        static string ordersLocation = "C:/Users/Nevena/Downloads/webProjekat/webProjekat/orders.xml";
        static string reviewsLocation = "C:/Users/Nevena/Downloads/webProjekat/webProjekat/reviews.xml";
        static string usersLocation = "C:/Users/Nevena/Downloads/webProjekat/webProjekat/users.xml";

        public static List<User> RetrieveAllUsers()
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<User>));
            using (var str = new StreamReader(usersLocation))
            {
                return (List<User>)xmlSerializ.Deserialize(str);
            }
        }
        public static void AddAllUsers(List<User> list)
        {
            User nevena = new User("nevena", "nevena", "Nevena", "Culibrk", Gender.Female, "culibrk.nevena@gmail.com", new DateTime(2001, 04, 12), Role.Administrator, false);
            User marko = new User("marko", "marko", "Marko", "Markovic", Gender.Male, "markovic.marko@gmail.com", new DateTime(1999, 11, 23), Role.Customer, false);
            User jovana = new User("jovana", "jovana", "Jovana", "Jankovic", Gender.Female, "jankovic.jovana@gmail.com", new DateTime(1995, 10, 20), Role.Seller, false);
            User nina = new User("nina", "nina", "Nina", "Ninkovic", Gender.Female, "ninkovic.ninc@gmail.com", new DateTime(1999, 03, 17), Role.Administrator, false);
            User milan = new User("milan", "milan", "Milan", "Milanovic", Gender.Male, "milanovic.milan@gmail.com", new DateTime(1995, 12, 16), Role.Customer, false);
            User jelena = new User("jelena", "jelena", "Jelena", "Jelenic", Gender.Female, "jelenic.jelena@gmail.com", new DateTime(1996, 02, 12), Role.Seller, false);
            //list.Add(nevena);
            //list.Add(marko);
            //list.Add(jovana);
            //list.Add(nina);
            //list.Add(milan);
            //list.Add(jelena);

            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<User>));
            using (var str = File.Create(usersLocation))
            {
                xmlSerializ.Serialize(str, list);
            }
        }
        public static User FindUser(string user)
        {
            foreach (User u in RetrieveAllUsers())
                if (u.Username == user)
                    return u;
            return null;
        }
        public static bool UserExists(User user)
        {
            foreach (User u in RetrieveAllUsers())
                if (u.Username == user.Username && u.Password == user.Password)
                    return true;
            return false;
        }
        public static bool AddUser(User user)
        {
            List<User> users = RetrieveAllUsers();
            foreach (User u in users)
                if (u.Username == user.Username)
                {
                    return false;
                }

            users.Add(user);
            AddAllUsers(users);
            return true;
        }
        public static bool RemoveUser(User user)
        {
            List<User> users = RetrieveAllUsers();
            foreach (User u in users)
                if (u.Username == user.Username)
                {
                    users.Remove(u);
                    AddAllUsers(users);
                    return true;
                }
            return false;
        }
        public static bool ChangeUser(User user)
        {
            List<User> users = RetrieveAllUsers();
            foreach (User u in users)
                if (u.Username == user.Username)
                {
                    RemoveUser(u);
                    AddUser(user);
                    return true;
                }
            return false;
        }

        public static List<Product> RetrieveAllProducts()
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Product>));
            using (var str = new StreamReader(productsLocation))
            {
                return (List<Product>)xmlSerializ.Deserialize(str);
            }
        }
        public static void AddAllProducts(List<Product> products)
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Product>));
            using (var str = File.Create(productsLocation))
            {
                xmlSerializ.Serialize(str, products);
            }
        }
        public static Product FindProduct(string product)
        {
            foreach (Product p in RetrieveAllProducts())
                if (p.Name == product)
                    return p;
            return null;
        }
        public static bool AddProduct(Product product)
        {
            List<Product> products = RetrieveAllProducts();
            foreach (Product p in products)
                if (product.Name == p.Name)
                {
                    return false;
                }

            products.Add(product);
            AddAllProducts(products);
            return true;
        }
        public static bool RemoveProduct(Product p)
        {
            List<Product> products = RetrieveAllProducts();
            foreach (Product product in products)
                if (p.Name == product.Name)
                {
                    products.Remove(product);
                    AddAllProducts(products);
                    return true;
                }
            return false;
        }
        public static bool ChangeProduct(Product p)
        {
            List<Product> products = RetrieveAllProducts();
            foreach (Product product in products)
                if (p.Name == product.Name)
                {
                    RemoveProduct(product);
                    AddProduct(p);
                    return true;
                }
            return false;
        }

        public static List<Product> RetrieveAllFavoriteProducts()
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Product>));
            using (var str = new StreamReader(favoriteProductsLocation))
            {
                return (List<Product>)xmlSerializ.Deserialize(str);
            }
        }
        public static void AddAllFavoriteProducts(List<Product> products)
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Product>));
            using (var str = File.Create(favoriteProductsLocation))
            {
                xmlSerializ.Serialize(str, products);
            }
        }
        public static Product FindFavoriteProduct(string product)
        {
            foreach (Product p in RetrieveAllFavoriteProducts())
                if (p.Name == product)
                    return p;
            return null;
        }
        public static bool AddFavoriteProduct(Product product)
        {
            List<Product> products = RetrieveAllFavoriteProducts();
            foreach (Product p in products)
                if (product.Name == p.Name)
                {
                    return false;
                }

            products.Add(product);
            AddAllFavoriteProducts(products);
            return true;
        }
        public static bool RemoveFavoriteProduct(Product p)
        {
            List<Product> products = RetrieveAllFavoriteProducts();
            foreach (Product product in products)
                if (p.Name == product.Name)
                {
                    products.Remove(product);
                    AddAllFavoriteProducts(products);
                    return true;
                }
            return false;
        }

        public static bool ChangeFavoriteProduct(Product p)
        {
            List<Product> products = RetrieveAllFavoriteProducts();
            foreach (Product product in products)
                if (p.Name == product.Name)
                {
                    RemoveFavoriteProduct(product);
                    AddFavoriteProduct(p);
                    return true;
                }
            return false;
        }

        public static List<Order> RetrieveAllOrders()
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Order>));
            using (var str = new StreamReader(ordersLocation))
            {
                return (List<Order>)xmlSerializ.Deserialize(str);
            }
        }
        public static void AddAllOrders(List<Order> orders)
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Order>));
            using (var str = File.Create(ordersLocation))
            {
                xmlSerializ.Serialize(str, orders);
            }
        }
        public static Order FindOrder(string order)
        {
            foreach (Order o in RetrieveAllOrders())
                if (o.Product == order)
                    return o;
            return null;
        }
        public static bool AddOrder(Order order)
        {
            List<Order> orders = RetrieveAllOrders();
            foreach (Order o in orders)
                if (o.Product == order.Product)
                {
                    return false;
                }

            orders.Add(order);
            AddAllOrders(orders);
            return true;
        }
        public static bool RemoveOrder(Order order)
        {
            List<Order> orders = RetrieveAllOrders();
            foreach (Order o in orders)
                if (o.Product == order.Product)
                {
                    orders.Remove(o);
                    AddAllOrders(orders);
                    return true;
                }
            return false;
        }
        public static bool ChangeOrder(Order order)
        {
            List<Order> orders = RetrieveAllOrders();
            foreach (Order o in orders)
                if (o.Product == order.Product)
                {
                    RemoveOrder(o);
                    AddOrder(order);
                    return true;
                }
            return false;
        }

        public static List<Review> RetrieveAllReviews()
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Review>));
            using (var str = new StreamReader(reviewsLocation))
            {
                return (List<Review>)xmlSerializ.Deserialize(str);
            }
        }
        public static void AddAllReviews(List<Review> reviews)
        {
            XmlSerializer xmlSerializ = new XmlSerializer(typeof(List<Review>));
            using (var str = File.Create(reviewsLocation))
            {
                xmlSerializ.Serialize(str, reviews);
            }
        }
        public static bool AddReview(Review review)
        {
            List<Review> reviews = RetrieveAllReviews();

            reviews.Add(review);
            AddAllReviews(reviews);
            return true;
        }
        public static bool RemoveReview(Review review)
        {
            List<Review> reviews = RetrieveAllReviews();
            foreach (Review r in reviews)
                if (r.Product == review.Product && r.Username == review.Username)
                {
                    reviews.Remove(r);
                    AddAllReviews(reviews);
                    return true;
                }
            return false;
        }
        public static bool ChangeReview(Review review)
        {
            List<Review> reviews = RetrieveAllReviews();
            foreach (Review r in reviews)
                if (r.Product == review.Product && r.Username == review.Username)
                {
                    RemoveReview(r);
                    AddReview(review);
                    return true;
                }
            return false;
        }
        public static Review FindReview(string review)
        {
            foreach (Review r in RetrieveAllReviews())
                if (r.Title == review)
                    return r;
            return null;
        }
    }
}