using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using UploadImage.Data;
using UploadImage.Models;

namespace UploadImage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbConteext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ApplicationDbConteext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> ProductList = _db.Products.Include(p => p.ProductsImages).ToList();

            //foreach (var product in ProductList)
            //{
            //    product.ProductsImages = _db.PrroductImageUrls
            //        .Where(p => p.ProductId == product.Id)
            //        .ToList();
            //}
            return View(ProductList);
        }

        public IActionResult Upsert(int? id)
        {

            if(id == null || id == 0)
            {
                return View();
            }
            else
            {
                Product? product = _db.Products.FirstOrDefault(x => x.Id == id);

                Product? product1 = _db.Products.Include(p => p.ProductsImages).FirstOrDefault(x => x.Id == id);   

                return View(product1);
            }            
        }


        [HttpPost]
        public IActionResult Upsert(Product product, List<IFormFile>? files)
        {
            if(ModelState.IsValid)
            {

                if (product.Id == 0)
                {
                    _db.Products.Add(product);
                }
                else
                {
                    _db.Products.Update(product);
                }

                _db.SaveChanges();


                string rootPtah = _webHostEnvironment.WebRootPath;
                if(files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"Images\Product-" + product.Id;
                        string finalPath = Path.Combine(rootPtah, productPath);

                        if(!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImageUrl productImagee = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = product.Id,
                        };


                        if(product.ProductsImages == null)
                        {
                            product.ProductsImages = new List<ProductImageUrl>(); 
                        }

                        product.ProductsImages.Add(productImagee);
                        _db.PrroductImageUrls.Add(productImagee);
                        _db.SaveChanges();
                    }


                    //    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //    string productPath = Path.Combine(rootPtah, @"Images\Products");

                    //    if(!string.IsNullOrEmpty(product.Url))
                    //    {
                    //        var oldImagePath = Path.Combine(rootPtah, product.Url.TrimStart('\\'));
                    //        if (System.IO.File.Exists(oldImagePath))
                    //        {
                    //            System.IO.File.Delete(oldImagePath);
                    //        }
                    //    }

                    //    using ( var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    //    {
                    //        file.CopyTo(fileStream);
                    //    }

                    //    product.Url = @"Images\Products\" + fileName;
                }               
                return RedirectToAction("Index");
            }
            
            return View();
        }



        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}