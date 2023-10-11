using Lab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace Lab2.Areas.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_admin)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment host;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment _host)
        {
            this.db = db;
            host = _host;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageIndex = page == null ? 1 : (int)page;
            var listProduct = db.Products.Include(x => x.Category).ToList();
            int pageSum = listProduct.Count() / pageSize + (listProduct.Count() % pageSize > 0 ? 1 : 0);
            ViewBag.PageSum = pageSum;
            ViewBag.PageIndex = pageIndex;
            return View(listProduct.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
        public IActionResult Search(string name, int? page)
        {
            if (page != null)
            {
                return RedirectToAction("Index", new { page = (int)page });
            }
            string nameF = name?.Replace(" ", "").ToLower();
            var listProduct = db.Products.Include(x => x.Category)
                .Where(product => product.Name.Replace(" ", "").ToLower().Contains(nameF)).ToList();
            if (listProduct.Count() == 0 || string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Name = name;
            ViewBag.Page= page;
            return View(listProduct);
        }    
        public IActionResult Create()
        {
            var listCategory = db.Categories.Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            ViewBag.ListCategory = listCategory;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product, IFormFile image)
        {
            int pageSize = 5;
           
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string path = Path.Combine(host.WebRootPath, @"images/products");
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }
                    product.ImageUrl = @"images/products/" + filename;
                }
                db.Products.Add(product);
                db.SaveChanges();
                var listProduct = db.Products.Include(x => x.Category).ToList();
                int pageSum = listProduct.Count() / pageSize + (listProduct.Count() % pageSize > 0 ? 1 : 0);
                TempData["Succses"] = "Create product succses";

                return RedirectToAction("Index",new {page=pageSum});
            }
            var listCategory = db.Categories.Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            ViewBag.ListCategory = listCategory;
            return View();
        }
        public IActionResult Edit(int id,string nameF,int? page)
        {
            var listCategory = db.Categories.Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            var objProduct = db.Products.Find(id);
            if (objProduct == null)
                return NotFound();
            ViewBag.Name = nameF;
            ViewBag.Page = page;
            ViewBag.ListCategory = listCategory;
            return View(objProduct);
        }
        [HttpPost]
        public IActionResult Edit(Product product, IFormFile image,int? page,string nameF)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string path = Path.Combine(host.WebRootPath, @"images/products");
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }
                    product.ImageUrl = @"images/products/" + filename;
                }
                db.Products.Update(product);
                db.SaveChanges();
                TempData["Succses"] = "Product update succses";
                if (page != null)
                    return RedirectToAction("Index", new { page = (int)page});
                else
                    return RedirectToAction("Search", new { name = nameF });
            }
            var listCategory = db.Categories.Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            ViewBag.ListCategory = listCategory;
            return View();
        }
        public IActionResult Delete(int id)
        {
            var objProduct = db.Products.Find(id);
            if (objProduct == null)
                return NotFound();
            if (!String.IsNullOrEmpty(objProduct.ImageUrl))
            {
                var oldFilePath = Path.Combine(host.WebRootPath, objProduct.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }
            db.Products.Remove(objProduct);
            db.SaveChanges();
            TempData["Succses"] = "Delete product succses";
            return RedirectToAction("Index");
        }
    }
}
