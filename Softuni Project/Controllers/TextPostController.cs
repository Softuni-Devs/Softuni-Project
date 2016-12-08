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

        public bool IsUserAuthorizedToEdit(TextPost textpost)
        {
            //Roles need to be added!
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

                    this.AddNotification("You have successfully created a Post.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");

                }
                
            }
            return View(post);

        }

        //
        // GET: TextPost/Edit
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
        //POST: Article/Edit
        [HttpPost]
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

                    return RedirectToAction("Index");

                }
            }
            return View(model);
        }
        //
        // GET: TextPost/Delete
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

                return RedirectToAction("Index");

            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostComment(SubmitCommentModel commentModel)
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
                        Content = commentModel.Comment,
                        TextPostId = commentModel.TextPostId,
                    });

                    db.SaveChanges();

                    var viewModel = new CommentViewModel { AuthorName = username, Content = commentModel.Comment };
                    //return View("_CommentPartial", viewModel);


                    //Redirect to the current page
                    return RedirectToAction("Details", new {id = commentModel.TextPostId});

                }
            }

            return RedirectToAction("Details", new { id = commentModel.TextPostId });
            //new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, ModelState.Values.First().ToString());
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
