using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using webProjekat.Models;

namespace webProjekat.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult MyProfile()
        {
            return View((User)Session["user"]);
        }
        public ActionResult ChangeProfileInformation(User user, string userGender)
        {
            User u = (User)Session["user"];
            if (user.FirstName == null || user.LastName == null || user.Email == null || !user.Email.Contains("@"))
            {
                ViewBag.ErrorMessage = "Invalid input";
                return View("MyProfile", u);
            }
            AllData.RemoveUser(u);
            u.FirstName = user.FirstName;
            u.LastName = user.LastName;
            u.Email = user.Email;
            if (user.Birthday != DateTime.MinValue && user.Birthday < DateTime.Now)
                u.Birthday = user.Birthday;

            if (userGender != "")
            {
                Gender g;
                Enum.TryParse(userGender, out g);
                u.Gender = g;
            }
            AllData.AddUser(u);
            Session["user"] = u;

            return RedirectToAction("AllProducts", "Home");
        }

        public ActionResult Favorites()
        {
            User user = (User)Session["user"];

            List<Product> favoriteProducts = user.FavoriteProducts;

            return View(favoriteProducts);
        }

        public ActionResult MyOrders()
        {
            User user = (User)Session["user"];

            List<Order> orders = user.Orders;

            return View(orders);
        }

        public ActionResult CancelOrder(string name)
        {
            User user = (User)Session["user"];

            Order order = user.Orders.FirstOrDefault(o => o.Product == name && o.Status == Status.Active);

            if (order != null)
            {
                Product product = AllData.FindProduct(order.Product);

                if (product != null)
                {
                    product.Quantity++;
                    AllData.ChangeProduct(product);
                }

                order.Status = Status.Cancelled;
                AllData.ChangeOrder(order);

                AllData.ChangeUser(user);
            }

            return RedirectToAction("MyOrders");
        }

        public ActionResult ExecuteOrder(string name)
        {
            User user = (User)Session["user"];

            Order order = user.Orders.FirstOrDefault(o => o.Product == name && o.Status == Status.Active);

            if (order != null)
            {
                Product product = AllData.FindProduct(order.Product);

                if (product != null)
                {
                    if (product.Quantity >= order.Quantity)
                    {
                        product.Quantity--; ;

                        AllData.ChangeProduct(product);

                        if (product.Quantity == 0)
                        {
                            AllData.RemoveProduct(product);
                            product.Deleted = true;
                            AllData.AddProduct(product);
                        }
                        order.Status = Status.Executed;
                        AllData.ChangeOrder(order);
                        AllData.ChangeUser(user);
                    }
                }
            }

            return RedirectToAction("MyOrders");
        }

        public ActionResult AddReview()
        {
            return View();
        }

        public ActionResult AddReviewRequest(Review review, HttpPostedFileBase imageFile, string name)
        {
            User user = (User)Session["user"];
            review.Username = user.Username;

            if (review.Title == null || review.Content == null)
            {
                ViewBag.ErrorMessage = "Invalid input";
                return View("AddReview");
            }

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageName);
                imageFile.SaveAs(imagePath);

                review.Image = "/Images/" + imageName;
            }

            Order executedOrder = user.Orders.FirstOrDefault(o => o.Status == Status.Executed);

            if (executedOrder != null)
            {
                Product product = AllData.FindProduct(executedOrder.Product);

                if (product != null)
                {
                    Review newReview = new Review
                    {
                        Product = product.Name,
                        Username = review.Username,
                        Title = review.Title,
                        Content = review.Content,
                        Image = review.Image,
                        Accepted = false,
                        Declined = false,
                    };

                    product.Reviews.Add(newReview);
                    AllData.AddReview(newReview);
                    AllData.ChangeProduct(product);

                    AllData.ChangeUser(user);

                    return RedirectToAction("AllProducts", "Home");
                }
            }

            return View("AddReview");
        }
    }
}