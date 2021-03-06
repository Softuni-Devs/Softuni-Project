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


        public ActionResult ListAll(string sortOrder, int? id)
        {
          
            ViewBag.TitleSortParm = "name_order";
            ViewBag.ScoreSortParm = "score_order";
            ViewBag.DateSortParm = "new_posts_first";
            using (var db = new BlogDbContext())
            {
                var posts = db.TextPosts.
                     Include(a => a.Author);
                     
                switch (sortOrder)
                {
                  
                    case "new_posts_first":
                       posts = posts.OrderByDescending(s => s.DatePosted);
                        
                        break;
                    case "name_order":
                        posts= posts.OrderBy(s => s.Title);
                        break;
 
                    case "score_order":
                      posts  = posts.OrderByDescending(s => s.Score);
                        break;
                    default:
                        posts = posts.OrderBy(s => s.DatePosted);
                        break;
                }
               

                var dbCategories = db.Categories.ToList();
                Dictionary<int,string> categories = new Dictionary<int, string>();

                foreach (var category in dbCategories)
                {
                    categories[category.Id] = category.Name;

                }

                ViewBag.Categories = categories;

                if (id != null)
                {
                    //The user clicked on a category, show only the posts in the same one

                    var categoryPosts = db.TextPosts.Where(t => t.CategoryId == id).Include(a => a.Author);

                    ViewBag.TitleSortParm = "name_order";
                    ViewBag.ScoreSortParm = "score_order";
                    ViewBag.DateSortParm = "new_posts_first";

                    switch (sortOrder)
                    {

                        case "new_posts_first":
                            categoryPosts = categoryPosts.OrderByDescending(s => s.DatePosted);

                            break;
                        case "name_order":
                            categoryPosts = categoryPosts.OrderBy(s => s.Title);
                            break;

                        case "score_order":
                            categoryPosts = categoryPosts.OrderByDescending(s => s.Score);
                            break;
                        default:
                            categoryPosts = categoryPosts.OrderBy(s => s.DatePosted);
                            break;
                    }



                    return View(categoryPosts.ToList());
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {

                var article = database.TextPosts
                    .Where(p => p.Id == id)
                    .Include(p => p.Author)
                    .Include(p => p.Comments.Select(c => c.Author))
                    .First();

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }
      
        [Authorize]
        public ActionResult Create()
        {
            //check if we have error messages from our last attempt to create a post
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"].ToString();
            }


            using (var databse = new BlogDbContext())
            {
                var model = new TextPostViewModel();
                model.Categories= databse.Categories
                    .OrderBy(c => c.Name)
                    .ToList();
                return View(model);
            }
                
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(TextPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var authorId = db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id;
                    var textPost = new TextPost(authorId, model.Title, model.Content, model.CategoryId);

                    textPost.DatePosted = DateTime.Now;
                    db.TextPosts.Add(textPost);
                    db.SaveChanges();

                    this.AddNotification("You have successfully created a Post.", NotificationType.SUCCESS);
                    TempData["message"] = null;
                    return RedirectToAction("Index");

                }

            }
            else
            {
                //the create process failed, this message will be displayed 
                TempData["message"] = "Please make sure the title and the content of your post are not empty.";
                return RedirectToAction("Create");

            }
            return View(model);

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
                model.CategoryId = textPost.CategoryId;
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();


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
                    textPost.CategoryId = model.CategoryId;


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
                    .Include(c=>c.Category)
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
        public ActionResult LikePost(int? id, TextPost model)
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

                return RedirectToAction("ListAll", new { id = currentPostID.CategoryId });

            }

        }

    }
}
