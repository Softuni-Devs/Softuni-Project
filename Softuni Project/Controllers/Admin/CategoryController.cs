using Softuni_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Softuni_Project.Controllers.Admin
{
    public class CategoryController : Controller
    {
        //
        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        //GET: Category/List
        public ActionResult List()
        {
            using(var database = new BlogDbContext())
            {
                var categories = database.Categories
                    .ToList();
                return View(categories);
            }
        }

        //
        //GET: Category/Create
        public ActionResult  Create()
        {
            return View();
        }
        //
        // POST: Category/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    database.Categories.Add(category);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(category);
        }

        //
        // GET: Category/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {

                var category = database.Categories
                    .FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    return HttpNotFound();
                }


                return View(category);
            }
        }

        //
        // POST: Category/Edit
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {

                    database.Entry(category).State = EntityState.Modified;
                    database.SaveChanges();

                    
                    return RedirectToAction("Index");
                }
            }
            
            return View(category);
        }

        //
        // GET: Category/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var category = database.Categories
                  .FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        //
        // POST: Category/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {


            using (var database = new BlogDbContext())
            {

                var category = database.Categories
                   .FirstOrDefault(c => c.Id == id);

                var categoryTextPosts = category.TextPosts.ToList();

                foreach (var article in categoryTextPosts)
                {
                    database.TextPosts.Remove(article);
                }


                database.Categories.Remove(category);
                database.SaveChanges();


                return RedirectToAction("Index");
            }
        }
    }
}