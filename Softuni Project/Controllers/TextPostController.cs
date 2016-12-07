using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Softuni_Project.Models;

namespace Softuni_Project.Controllers
{
    public class TextPostController : Controller
    {
        // GET: TextPost
        public ActionResult Index()
        {


            return RedirectToAction("ListAll");

        }

        public ActionResult ListAll()
        {

            using (var db  = new BlogDbContext())
            {
                var posts = db.TextPosts.
                    Include(a => a.Author)
                    .ToList();

                return View(posts);
            }
            
           
        }


        //
        // GET: TextPost/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var textPosts = database.TextPosts
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (textPosts == null)
                {
                    return HttpNotFound();
                }

                return View(textPosts);
            }
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TextPost post)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var authorId = db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id;
                    post.AuthorId = authorId;


                    db.TextPosts.Add(post);
                    db.SaveChanges();


                    return RedirectToAction("Index");

                }
                
            }
            return View(post);

        }


        [HttpPost]
        public ActionResult LikePost(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var currentUserID = db.Users.First(u => u.UserName == this.User.Identity.Name).Id;

                var currentPostID = db.TextPosts.First(p => p.Id == id);

                
                if (!currentPostID.UsersLikesIDs.Contains(currentUserID))
                {
                    currentPostID.UsersLikesIDs += currentUserID;
                    currentPostID.Score += 1;
                    db.SaveChanges();


                }


            }



            return RedirectToAction("ListAll");
        }

    }
}
