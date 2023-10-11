using Lab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace Lab2.Areas.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            this.db = db;
        }
        public IActionResult Index(int? page)
        {
            int pageSize = 9;
            int pageIndex = page == null ? 1 : (int)page;
            var listProduct = db.Products.ToList();
            int pageSum = listProduct.Count() / pageSize + (listProduct.Count() % pageSize > 0 ? 1 : 0);
            ViewBag.PageSum = pageSum;
            ViewBag.PageIndex = pageIndex;
            return View(listProduct.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
        public IActionResult Detail(int id,string name,int? page)
        {
            var objProduct = db.Products.Find(id);
            if (objProduct == null)
                return NotFound();
            ViewBag.Name = name;
            ViewBag.Page = page;
            ViewBag.ListProduct = db.Products.Where(w => w.CategoryId == objProduct.CategoryId && w.Id != id).Take(5).ToList();
            return View(objProduct);
        }
        public IActionResult Search(string name,int? page)
        {
            if (page != null)
            {
                return RedirectToAction("Index", new {page = (int)page});
            }
            string nameF = name?.Replace(" ", "").ToLower();
            var listProduct = db.Products
                .Where(product => product.Name.Replace(" ", "").ToLower().Contains(nameF))
                .ToList();
            if (listProduct.Count() == 0 || string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Name = name;
            return View(listProduct);
        }
    }
}
