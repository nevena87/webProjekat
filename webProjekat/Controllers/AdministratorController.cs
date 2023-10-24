using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webProjekat.Models;

namespace webProjekat.Controllers
{
    public class AdministratorController : Controller
    {
        public ActionResult Products()
        {
            List<Product> products = AllData.RetrieveAllProducts();
            return View(products);
        }

        public ActionResult Orders()
        {
            List<Order> orders = AllData.RetrieveAllOrders();
            return View(orders);
        }

        public ActionResult ChangeOrderStatus(string name, Status status)
        {
            User user = (User)Session["user"];
            Order order = AllData.FindOrder(name);
            if (order == null)
            {
                ViewBag.ErrorMessage = "Order not found.";
            }
            else
            {
                if (status == Status.Executed || status == Status.Cancelled)
                {
                    if (order.Status == Status.Active)
                    {
                        if (status == Status.Cancelled)
                        {
                            Product product = AllData.FindProduct(order.Product);
                            if (product != null)
                            {
                                product.Quantity += order.Quantity;
                                AllData.ChangeProduct(product);
                            }
                        }

                        order.Status = status;
                        AllData.ChangeOrder(order);

                        Order userOrder = user.Orders.FirstOrDefault(o => o.Product == order.Product);
                        if (userOrder != null)
                        {
                            userOrder.Status = status;
                            AllData.ChangeUser(user);
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid order status. Only active orders can be executed or cancelled.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid order status.";
                }
            }
            AllData.ChangeUser(user);
            return RedirectToAction("Orders");
        }

        public ActionResult SortProducts(string type, string by)
        {
            List<Product> products = AllData.RetrieveAllProducts();
            switch (type)
            {
                case "Descending":
                    if (by == "Name")
                        products.Sort(delegate (Product a, Product b) { return b.Name.CompareTo(a.Name); });
                    else if (by == "Price")
                        products.Sort(delegate (Product a, Product b) { return b.Price.CompareTo(a.Price); });
                    else if (by == "ProductPlacementDate")
                        products.Sort(delegate (Product a, Product b) { return b.ProductPlacementDate.CompareTo(a.ProductPlacementDate); });
                    break;
                case "Ascending":
                    if (by == "Name")
                        products.Sort(delegate (Product a, Product b) { return a.Name.CompareTo(b.Name); });
                    else if (by == "Price")
                        products.Sort(delegate (Product a, Product b) { return a.Price.CompareTo(b.Price); });
                    else if (by == "ProductPlacementDate")
                        products.Sort(delegate (Product a, Product b) { return a.ProductPlacementDate.CompareTo(b.ProductPlacementDate); });
                    break;
            }
            return View("Products", products);
        }

        public ActionResult FilterProduct(string status)
        {
            List<Product> products = AllData.RetrieveAllProducts();
            List<Product> filteredProducts = new List<Product>();

            if (Enum.TryParse(status, out StatusAUn productStatus))
            {
                filteredProducts = products.Where(p => p.Status == productStatus).ToList();
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid status.";
            }

            return View("Products", filteredProducts);
        }

        public ActionResult Users()
        {
            List<User> users = AllData.RetrieveAllUsers();
            List<User> myUsers = new List<User>();
            foreach (User u in users)
            {
                if (u.Role == Role.Seller || u.Role == Role.Customer)
                    myUsers.Add(u);
            }
            return View(myUsers);
        }

        public ActionResult SearchUsers(string birthdayFrom, string birthdayTo, string firstName, string lastName, string role)
        {
            List<User> users = AllData.RetrieveAllUsers();
            List<User> searchedUsers = new List<User>();

            DateTime.TryParse(birthdayFrom, out DateTime fromDate);
            DateTime.TryParse(birthdayTo, out DateTime toDate);
            Enum.TryParse(role, out Role selectedRole);

            foreach (var user in users)
            {
                bool matchFound = true;

                if (!string.IsNullOrEmpty(firstName) && !user.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase))
                    matchFound = false;

                if (!string.IsNullOrEmpty(lastName) && !user.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                    matchFound = false;

                if (fromDate != DateTime.MinValue && user.Birthday < fromDate)
                    matchFound = false;

                if (toDate != DateTime.MinValue && user.Birthday > toDate)
                    matchFound = false;

                if (!string.IsNullOrEmpty(role))
                {
                    if (selectedRole == Role.Customer && user.Role != Role.Customer)
                        matchFound = false;

                    if (selectedRole == Role.Seller && user.Role != Role.Seller)
                        matchFound = false;
                }

                if (user.Role == Role.Administrator)
                    matchFound = false;

                if (matchFound)
                    searchedUsers.Add(user);
            }

            return View("Users", searchedUsers);
        }

        public ActionResult SortUsers(string type, string by)
        {
            List<User> users = AllData.RetrieveAllUsers();
            List<User> myUsers = new List<User>();
            foreach (User u in users)
            {
                if (u.Role == Role.Customer || u.Role == Role.Seller)
                {
                    myUsers.Add(u);
                }
            }
            switch (type)
            {
                case "Descending":
                    if (by == "FirstName")
                        myUsers.Sort(delegate (User a, User b) { return b.FirstName.CompareTo(a.FirstName); });
                    else if (by == "Birthday")
                        myUsers.Sort(delegate (User a, User b) { return b.Birthday.CompareTo(a.Birthday); });
                    else if (by == "Role")
                        myUsers.Sort(delegate (User a, User b) { return b.Role.CompareTo(a.Role); });
                    break;
                case "Ascending":
                    if (by == "FirstName")
                        myUsers.Sort(delegate (User a, User b) { return a.FirstName.CompareTo(b.FirstName); });
                    else if (by == "Birthday")
                        myUsers.Sort(delegate (User a, User b) { return a.Birthday.CompareTo(b.Birthday); });
                    else if (by == "Role")
                        myUsers.Sort(delegate (User a, User b) { return a.Role.CompareTo(b.Role); });
                    break;
            }
            return View("Users", myUsers);
        }

        public ActionResult Delete(string username, string name)
        {
            User user = AllData.FindUser(username);
            List<Product> products = AllData.RetrieveAllProducts().Where(p => p.Name == name).ToList();
            if (user != null)
            {
                if (user.Role == Role.Seller)
                {
                    foreach (Product product in products)
                    {
                        List<Product> favoriteProducts = AllData.RetrieveAllFavoriteProducts().Where(p => p.Name == product.Name).ToList();
                        foreach(Product p in favoriteProducts)
                        {
                            user.FavoriteProducts.Remove(product);
                            AllData.RemoveFavoriteProduct(product);
                            product.Deleted = true;
                            AllData.AddFavoriteProduct(product);
                        }
                        List<Order> activeOrders = AllData.RetrieveAllOrders().Where(o => o.Product == product.Name && o.Status == Status.Active).ToList();
                        foreach (Order order in activeOrders)
                        {
                            user.Orders.Remove(order);
                            AllData.RemoveOrder(order);
                            order.Deleted = true;
                            AllData.AddOrder(order);
                        }
                        user.ReleasedProducts.Remove(product.Name);
                        AllData.RemoveProduct(product);
                        product.Deleted = true;
                        AllData.AddProduct(product);
                    }

                    AllData.RemoveUser(user);
                    user.Deleted = true;
                    AllData.AddUser(user);
                }
                else if (user.Role == Role.Customer)
                {
                    List<Order> activeOrders = AllData.RetrieveAllOrders().Where(o => o.Product == name && o.Status == Status.Active).ToList();
                    foreach (Order order in activeOrders)
                    {
                        user.Orders.Remove(order);
                        AllData.RemoveOrder(order);
                        order.Deleted = true;
                        AllData.AddOrder(order);

                        Product product = AllData.FindProduct(order.Product);
                        if (product != null)
                        {
                            product.Quantity++;
                            AllData.ChangeProduct(product);
                        }
                    }
                    AllData.RemoveUser(user);
                    user.Deleted = true;
                    AllData.AddUser(user);
                }
            }

            return RedirectToAction("Users");
        }

        public ActionResult DeleteProduct(string name)
        {
            User user = (User)Session["user"];
            Product product = AllData.FindProduct(name);
            List<Product> favoriteProducts = AllData.RetrieveAllFavoriteProducts();
            foreach (Product p in favoriteProducts)
            {
                if (p.Name == product.Name)
                {
                    p.Status = StatusAUn.Unavailable;
                    AllData.ChangeFavoriteProduct(p);
                }
            }
            List<Order> orders = AllData.RetrieveAllOrders().Where(o => o.Status == Status.Active).ToList();
            foreach (Order order in orders)
            {
                if (order.Product == product.Name)
                {
                    user.Orders.Remove(order);
                    AllData.RemoveOrder(order);
                    order.Deleted = true;
                    AllData.AddOrder(order);
                }
            }
            if (product.Status != StatusAUn.Unavailable)
            {
                user.ReleasedProducts.Remove(product.Name);
                AllData.RemoveProduct(product);
                product.Deleted = true;
                AllData.AddProduct(product);
            }

            AllData.ChangeUser(user);

            return RedirectToAction("Products");
        }

        public ActionResult AddSeller()
        {
            return View();
        }
        public ActionResult AddSellerRequest(User user, string gender)
        {
            if (user.FirstName == null || user.LastName == null
                || user.Username == null || user.Password == null
                || user.Email == null || !user.Email.Contains("@") || !user.Email.Contains(".")
                || user.Birthday > DateTime.Now
                || gender == ""
               )
            {
                ViewBag.ErrorMessage = "Input invalid.";
                return View("AddSeller");
            }
            Gender g;
            Enum.TryParse(gender, out g);
            user.Gender = g;
            user.ReleasedProducts = new List<string>();
            user.Deleted = false;
            user.Role = Role.Seller;
            if (!AllData.AddUser(user))
            {
                ViewBag.ErrorMessage = "User exists.";
                return View("AddSeller");
            }
            return RedirectToAction("Users");
        }

        public ActionResult ChangeSeller(string username)
        {
            return View(AllData.FindUser(username));
        }
        public ActionResult ChangeSellerRequest(User user, string gender)
        {
            User seller = AllData.FindUser(user.Username);
            AllData.RemoveUser(seller);
            if (user.FirstName == null || user.LastName == null
                || user.Username == null || user.Password == null
                || user.Email == null || !user.Email.Contains("@") || !user.Email.Contains(".")
                || gender == ""
                )
            {
                ViewBag.ErrorMessage = "Invalid input.";
                return View("ChangeSeller", seller);
            }
            Gender g;
            Enum.TryParse(gender, out g);
            seller.Gender = g;
            seller.FirstName = user.FirstName;
            seller.LastName = user.LastName;
            seller.Email = user.Email;
            seller.Birthday = user.Birthday;
            AllData.AddUser(seller);
            return RedirectToAction("Users");
        }

        public ActionResult ChangeProduct(string name)
        {
            return View(AllData.FindProduct(name));
        }
        public ActionResult ChangeProductRequest(Product product, string status, HttpPostedFileBase imageFile)
        {
            Product existingProduct = AllData.FindProduct(product.Name);
            AllData.RemoveProduct(existingProduct);
            if (product.Name == null || product.Price == 0 || product.Quantity == 0 || product.Description == null
                || product.ProductPlacementDate == null || product.City == null || status == "" )
            {
                ViewBag.ErrorMessage = "Invalid input.";
                return View("ChangeProduct", product);
            }
            StatusAUn s;
            Enum.TryParse(status, out s);
            existingProduct.Status = s;
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Description = product.Description;
            existingProduct.ProductPlacementDate = product.ProductPlacementDate;
            existingProduct.City = product.City;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageName);
                imageFile.SaveAs(imagePath);

                existingProduct.Image = "/Images/" + imageName;
            }
            AllData.AddProduct(existingProduct);
            return RedirectToAction("Products");

        }

        public ActionResult Reviews()
        {
            List<Review> allReviews = AllData.RetrieveAllReviews();
            return View(allReviews);
        }

        public ActionResult Accept()
        {
            List<Review> reviews = AllData.RetrieveAllReviews();

            foreach(Review r in reviews)
            {
                r.Accepted = true;
                AllData.ChangeReview(r);
            }

            return RedirectToAction("Reviews");
        }

        public ActionResult Decline()
        {
            List<Review> reviews = AllData.RetrieveAllReviews();

            foreach (Review r in reviews)
            {
                r.Declined = true;
                AllData.ChangeReview(r);
            }

            return RedirectToAction("Reviews", "Administrator");
        }
    }
}