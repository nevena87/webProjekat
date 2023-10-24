using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webProjekat.Models;

namespace webProjekat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //AllData.AddAllUsers(new List<User>());
            //AllData.AddAllProducts(new List<Product>());
            //AllData.AddAllOrders(new List<Order>());
            //AllData.AddAllFavoriteProducts(new List<Product>());
            //AllData.AddAllReviews(new List<Review>());
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult RegistrationRequest(User user, string usersGender)
        {
            if (user.FirstName == null || user.LastName == null || user.Username == null
                || user.Password == null || user.Email == null || !user.Email.Contains("@") || !user.Email.Contains(".") 
                || user.Email.Length < 5 || user.Birthday > DateTime.Now || usersGender == "")
            {
                ViewBag.ErrorMessage = "Invalid input.";
                return View("Registration");
            }
            Gender g;
            Enum.TryParse(usersGender, out g);
            user.Gender = g;
            user.Orders = new List<Order>();
            user.FavoriteProducts = new List<Product>();
            user.ReleasedProducts = new List<string>();
            user.Deleted = false;
            if (!AllData.AddUser(user))
            {
                ViewBag.ErrorMessage = "User exists.";
                return View("Registration");
            }
            return RedirectToAction("Index");
        }

        public ActionResult LoginRequest(User user)
        {
            User u = AllData.FindUser(user.Username);
            if (user.Username != null && user.Password != null && AllData.UserExists(user))
            {
                if (u.Deleted)
                {
                    ViewBag.ErrorMessage = "Your account has been deleted.";
                    return View("Index");
                }
                else
                {
                    Session["user"] = u;
                    return RedirectToAction("AllProducts");
                }
            }
            ViewBag.ErrorMessage = "Invalid";
            return View("Index");
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult AllProducts()
        {
            List<Product> list = AllData.RetrieveAllProducts();
            list.Sort(delegate (Product a, Product b) { return a.Name.CompareTo(b.Name); });
            return View(list);
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
            return View("AllProducts", products);
        }

        public ActionResult SearchProducts(string priceFrom, string priceTo, string city, string name)
        {
            List<Product> products = AllData.RetrieveAllProducts();
            if (city == "" && name == "" && priceTo == "" && priceFrom == "")
                return View("AllProducts", products);
            List<Product> productsName = new List<Product>();
            List<Product> productsPrice = new List<Product>();
            List<Product> productsCity = new List<Product>();
            double price1 = -1;
            double price2 = -1;
            double.TryParse(priceFrom, out price1);
            double.TryParse(priceTo, out price2);
            city = city.ToUpper();
            name = name.ToUpper();
            if (name != "")
                for (int i = 0; i < products.Count(); i++)
                {
                    if (products[i].Name.ToUpper().Contains(name))
                        productsName.Add(products[i]);
                }
            else
                productsName = products;
            if (price1 != 0 && price2 != 0)
                for (int i = 0; i < productsName.Count(); i++)
                {
                    if (productsName[i].Price >= price1 && productsName[i].Price <= price2)
                        productsPrice.Add(productsName[i]);
                }
            else
                productsPrice = productsName;
            if (city != "")
                for (int i = 0; i < productsPrice.Count(); i++)
                {
                    if (productsPrice[i].City.ToUpper().Contains(city))
                        productsCity.Add(productsPrice[i]);
                }
            else
                productsCity = productsPrice;

            return View("AllProducts", productsCity);
        }

        public ActionResult AddToFavorites(string name, HttpPostedFileBase imageFile)
        {
            User user = (User)Session["user"];

            Product product = AllData.FindProduct(name);

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageName);
                imageFile.SaveAs(imagePath);

                product.Image = "/Images/" + imageName;
            }

            Product existingProduct = user.FavoriteProducts.FirstOrDefault(p => p.Name == product.Name);

            if (existingProduct != null)
            {
                existingProduct.Quantity++;

                Product mainProduct = AllData.FindProduct(existingProduct.Name);
                if (mainProduct.Quantity == 0)
                {
                    existingProduct.Status = StatusAUn.Unavailable;
                }

                AllData.ChangeFavoriteProduct(existingProduct);
                AllData.ChangeUser(user);
            }
            else
            {
                Product newProduct = new Product
                {
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    Description = product.Description,
                    Image = product.Image,
                    ProductPlacementDate = product.ProductPlacementDate,
                    City = product.City
                };

                Product mainProduct = AllData.FindProduct(newProduct.Name);
                if (mainProduct.Quantity == 0)
                {
                    newProduct.Status = StatusAUn.Unavailable;
                }
                user.FavoriteProducts.Add(newProduct);
                AllData.AddFavoriteProduct(newProduct);
            }

            AllData.ChangeUser(user);

            return RedirectToAction("Favorites", "Customer");
        }

        public ActionResult OrderProduct(string name)
        {
            User user = (User)Session["user"];

            Product product = AllData.FindProduct(name);

            Order existingOrder = user.Orders.FirstOrDefault(o => o.Product == product.Name && o.Status == Status.Active);

            if (existingOrder != null)
            {
                existingOrder.Quantity++;
                user.Orders.Add(existingOrder);
                AllData.ChangeOrder(existingOrder);
            }
            else
            {
                Order newOrder = new Order
                {
                    Product = product.Name,
                    Quantity = 1,
                    Customer = user.Username,
                    OrderDate = DateTime.Now,
                    Status = Status.Active
                };

                user.Orders.Add(newOrder);
                AllData.AddOrder(newOrder);
            }

            product.Quantity--;

            AllData.ChangeUser(user);
            AllData.ChangeProduct(product);

            return RedirectToAction("MyOrders", "Customer");
        }

        public ActionResult ChangeReview(string title)
        {
            return View(AllData.FindReview(title));
        }

        public ActionResult ChangeReviewRequest(Review review, HttpPostedFileBase imageFile)
        {
            User user = (User)Session["user"];
            Review existingReview = AllData.FindReview(review.Title);

            if (existingReview == null || existingReview.Username != user.Username)
            {
                ViewBag.ErrorMessage = "You are not authorized to change this review.";
                return View("ChangeReview", existingReview);
            }

            if (review.Title == null || review.Content == null)
            {
                return View("ChangeReview", existingReview);
            }

            existingReview.Title = review.Title;
            existingReview.Content = review.Content;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageName);
                imageFile.SaveAs(imagePath);

                existingReview.Image = "/Images/" + imageName;
            }

            AllData.ChangeReview(existingReview);

            return RedirectToAction("AllProducts");
        }

        public ActionResult DeleteReview(string title)
        {
            Review review = AllData.FindReview(title);
            if(review != null)
            {
                AllData.RemoveReview(review);
                review.Deleted = true;
            }
            return RedirectToAction("AllProducts");
        }
    }
}