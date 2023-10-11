using Lab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace Lab2.Areas.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_admin)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db;
        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var listCategory = db.Categories.ToList();
            return View(listCategory);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["Succses"] = "Create succses";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int id)
        {
            var tl = db.Categories.Find(id);
            if (tl == null)
            {
                return NotFound();
            }
            return View(tl);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Update(category);
                //db.Entry<category>(ctg).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Succses"] = "Update Success";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            var tl = db.Categories.Find(id);
            if (tl == null)
                return NotFound();

            db.Categories.Remove(tl);
            db.SaveChanges();
            TempData["succses"] = "Delete Success";
            return RedirectToAction("Index");
        }
        
    }
}
