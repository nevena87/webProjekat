using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webProjekat.Models;

namespace webProjekat.Controllers
{
    public class SellerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProducts()
        {
            User user = (User)Session["user"];

            List<Product> products = RetrieveProductsBySeller().Where(p => !p.Deleted).ToList();

            return View(products);
        }
        public ActionResult AddProduct()
        {
            return View();
        }

        public ActionResult AddProductRequest(Product product, string status, HttpPostedFileBase imageFile)
        {
            if (product.Name != null && product.Price != 0 && product.Description != null && product.ProductPlacementDate != null && product.City != null && status != null && imageFile != null)
            {
                User seller = (User)Session["user"];
                StatusAUn s;
                Enum.TryParse(status, out s);
                product.Status = s;
                product.Deleted = false;
                product.Reviews = new List<Review>();

                string fileName = Path.GetFileName(imageFile.FileName);

                string imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);

                imageFile.SaveAs(imagePath);

                product.Image = "/Images/" + fileName;

                AllData.AddProduct(product);
                AllData.RemoveUser(seller);
                seller.ReleasedProducts.Add(product.Name);
                AllData.AddUser(seller);
                return RedirectToAction("MyProducts");
            }
            ViewBag.ErrorMessage = "Invalid input.";
            return RedirectToAction("AddProduct");

        }

        public ActionResult ChangeProduct(string name)
        {
            return View(AllData.FindProduct(name));
        }
        public ActionResult ChangeProductRequest(Product product, string status, HttpPostedFileBase imageFile)
        {
            Product existingProduct = AllData.FindProduct(product.Name);
            AllData.RemoveProduct(existingProduct);
            if (product.Name == null || product.Price == 0 || product.Description == null
                || product.ProductPlacementDate == null || product.City == null || status == "")
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

            if (product.Quantity > 0)
            {
                existingProduct.Status = StatusAUn.Available;
            }
            else
            {
                existingProduct.Status = StatusAUn.Unavailable;
            }

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageName);
                imageFile.SaveAs(imagePath);

                existingProduct.Image = "/Images/" + imageName;
            }

            AllData.AddProduct(existingProduct);
            return RedirectToAction("MyProducts");
        }

        public ActionResult DeleteProduct(string name)
        {
            User user = (User)Session["user"];
            Product product = AllData.FindProduct(name);

            List<Product> favoriteProducts = AllData.RetrieveAllFavoriteProducts();
            foreach (Product p in favoriteProducts)
            {
                if(p.Name == product.Name)
                {
                    p.Status = StatusAUn.Unavailable;
                    AllData.ChangeFavoriteProduct(p);

                }
            }

            List<Order> orders = AllData.RetrieveAllOrders().Where(o => o.Status == Status.Active).ToList();
            foreach (Order order in orders)
            {
                if(order.Product == product.Name)
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

            return RedirectToAction("MyProducts");
        }

        public ActionResult SortProducts(string by, string type)
        {
            List<Product> products = RetrieveProductsBySeller();
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
            return View("MyProducts", products);
        }

        private List<Product> RetrieveProductsBySeller()
        {
            List<Product> products = new List<Product>();
            User seller = (User)Session["user"];
            foreach (Product p in AllData.RetrieveAllProducts())
            {
                if (seller.ReleasedProducts.Contains(p.Name))
                    products.Add(p);
            }
            return products;
        }

        public ActionResult FilterProduct(string status)
        {
            List<Product> products = RetrieveProductsBySeller();
            List<Product> filteredProducts = new List<Product>();

            if (Enum.TryParse(status, out StatusAUn productStatus))
            {
                filteredProducts = products.Where(p => p.Status == productStatus).ToList();
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid status.";
            }

            return View("MyProducts", filteredProducts);
        }

    }
}