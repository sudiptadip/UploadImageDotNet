using Microsoft.AspNetCore.Mvc;
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
            List<Product> ProductList = _db.Products.ToList();
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
                return View(product);
            }            
        }

        [HttpPost]
        public IActionResult Upsert(Product product, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string rootPtah = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(rootPtah, @"Images\Products");

                    if(!string.IsNullOrEmpty(product.Url))
                    {
                        var oldImagePath = Path.Combine(rootPtah, product.Url.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using ( var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    product.Url = @"Images\Products\" + fileName;
                }

                if(product.Id == 0)
                {
                    _db.Products.Add(product);
                }
                else
                {
                    _db.Products.Update(product);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View();
        }



        [HttpPost]
        public IActionResult Create(Product obj)
        {
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