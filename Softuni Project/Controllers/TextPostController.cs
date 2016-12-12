using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Softuni_Project.Models;
using Microsoft.AspNet.Identity;
using Softuni_Project.Extensions;

namespace Softuni_Project.Controllers
{
    public class TextPostController : Controller
    {
        // GET: TextPost
        public ActionResult Index()
        {


            return RedirectToAction("ListAll");

        }
      
        public ActionResult ListAll(string sortOrder)
        {
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ScoreSortParm = sortOrder == "Score" ? "score_desc" : "Score";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            using (var db = new BlogDbContext())
            {
                var posts = db.TextPosts.
                     Include(a => a.Author);
                     
                switch (sortOrder)
                {
                    case "Date":
                        posts = posts.OrderBy(s => s.DatePosted);
                        break;
                    case "date_desc":
                       posts = posts.OrderByDescending(s => s.DatePosted);
                        break;
                    case "name_desc":
                        posts= posts.OrderByDescending(s => s.Title);
                        break;
                    case "Score":
                        posts = posts.OrderBy(s => s.Score);
                        break;
                    case "score_desc":
                      posts  = posts.OrderByDescending(s => s.Score);
                        break;
                    default:
                        posts = posts.OrderBy(s => s.Title);
                        break;
                }
                return View(posts.ToList());
            }
          
            
           
        }

        public bool IsUserAuthorizedToEdit(TextPost textpost)
        {
            
            bool IsAdmin = this.User.IsInRole("Admin");
            bool IsAuthor = textpost.IsAuthor(this.User.Identity.Name);
            return IsAdmin || IsAuthor;
        }
        //
        // GET: TextPost/Details
        public ActionResult Details(int? id)
        {

            if (!id.HasValue)
            {
                RedirectToAction("Index", "Home");
            }

            using (var database = new BlogDbContext())
            {
                var model = database.TextPosts
                            .Where(x => x.Id == id.Value)
                            .Select(TextPostDetailsVewModel.ToViewModel)
                            .FirstOrDefault();

                return View(model);


            }
        }
      
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
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

                    this.AddNotification("You have successfully created a Post.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");

                }
                
            }
            return View(post);

        }

        //
        // GET: TextPost/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {
                var textPost = database.TextPosts
                    .Where(a => a.Id == id)
                    .First();
                if (textPost == null)
                {
                    return HttpNotFound();
                }
                var model = new TextPostViewModel();
                model.Id = textPost.Id;
                model.Title = textPost.Title;
                model.Content = textPost.Content;

                return View(model);
            }
        }
        //
        //POST: TextPost/Edit
        [HttpPost]
        [Authorize]
        [ActionName("Edit")]
        public ActionResult Edit(TextPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var textPost = database.TextPosts
                        .FirstOrDefault(a => a.Id == model.Id);
                    if (!IsUserAuthorizedToEdit(textPost))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }
                    textPost.Title = model.Title;
                    textPost.Content = model.Content;


                    database.Entry(textPost).State = EntityState.Modified;
                    database.SaveChanges();
                    this.AddNotification("You have successfully edited your Post.", NotificationType.SUCCESS);

                    return RedirectToAction("Index");

                }
            }
            return View(model);
        }
        //
        // GET: TextPost/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {
                var textPost = database.TextPosts
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();
                if (textPost == null)
                {
                    return HttpNotFound();
                }
                return View(textPost);
            }
        }

        //
        //POST: TextPost/Delete
        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {
                var textPost = database.TextPosts
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();
                if (!IsUserAuthorizedToEdit(textPost))
                {

                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }


                if (textPost== null)
                {
                    return HttpNotFound();
                }

                database.TextPosts.Remove(textPost);
                database.SaveChanges();
                this.AddNotification("Post deleted.", NotificationType.INFO);

                return RedirectToAction("Index");

            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostComment(CommentViewModel commentModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var username = this.User.Identity.GetUserName();
                    var userId = this.User.Identity.GetUserId();
                    
                     db.Comments.Add(new Comment()
                    {
                        AuthorId = userId,
                        Content = commentModel.Content,
                        TextPostId = commentModel.TextPostId,
                    });

                    db.SaveChanges();

                    return RedirectToAction("Details", new {id = commentModel.TextPostId});
                }
            }

            return RedirectToAction("Details", new { id = commentModel.TextPostId });
            
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
                    this.AddNotification("Post liked!", NotificationType.SUCCESS);


                }


            }



            return RedirectToAction("ListAll");
        }

    }
}
