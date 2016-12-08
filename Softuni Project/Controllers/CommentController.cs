using Softuni_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Softuni_Project.Controllers
{
    public class CommentController : Controller
    {   
        //
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        //
        //GET: Comment/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {                                
                Comment comment = database.Comments
                    .Where(a => a.Id == id)
                    .First();
                if (comment == null)
                {
                    return HttpNotFound();
                }
                
                var model = new CommentViewModel();
                model.Id = comment.Id;               
                model.Content = comment.Content;
                return View(comment);
            }
        }

        //
        // POST: Comment/Edit       
        [HttpPost]       
        public ActionResult Edit(CommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {                                         
                    var comment  = database.Comments.FirstOrDefault(a => a.Id == model.Id);
                    if (!IsUserAuthorizedToEdit(comment))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }
     
                        comment.Content = model.Content;
                        database.Entry(comment).State = EntityState.Modified;
                        database.SaveChanges();

                    return RedirectToAction("Details", "TextPost", new { id = comment.TextPostId });
                }
            }
            return View(model);
        }

        //
        // GET: Comment/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {
                Comment comment = database.Comments
                    .Where(a => a.Id == id)                    
                    .First();
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
        }

        // POST: Comment/Delete
        [HttpPost, ActionName("Delete")]       
        public ActionResult DeleteConfirmed(int id)
        {                                        
            using (var database = new BlogDbContext())
            {
                Comment comment = database.Comments
                     .Where(a => a.Id == id)
                     .First();
                if (comment == null)
                {
                    return HttpNotFound();
                }
                if (!IsUserAuthorizedToEdit(comment))
                {

                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
           
                database.Comments.Remove(comment);
                database.SaveChanges();
                return RedirectToAction("Details","TextPost", new { id = comment.TextPostId });
               
            }
        }

        private bool IsUserAuthorizedToEdit(Comment comment)
        {
            bool IsAdmin = this.User.IsInRole("Admin");
            bool IsAuthor = comment.IsAuthor(this.User.Identity.Name);
            return IsAdmin || IsAuthor;
        }
    }
}