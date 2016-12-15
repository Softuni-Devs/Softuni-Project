using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Softuni_Project.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Softuni_Project.Controllers
{
    public class UserProfileController : Controller
    {
        // GET: UserProfile
        public ActionResult Index()
        {
            ViewBag.NumberOfPosts = NumberOfPosts();
            ViewBag.TotalScore = TotalScore();
            ViewBag.FullName = GetFullName();
            return View();
        }

        public string GetFullName()
        {
            using (var database = new BlogDbContext())
            {
                var userId = User.Identity.GetUserId();
                string fullName = database.Users.FirstOrDefault(c => c.Id == userId).FullName;
                return fullName;
            }
        }
        public int NumberOfPosts()
        {
            using (var database = new BlogDbContext())
            {
                var userId = User.Identity.GetUserId();
                int  NumberOfPost = database.TextPosts.Where(t => t.AuthorId == userId).Count();
                return NumberOfPost;
            }
        }
        public int TotalScore()
        {
            using (var database = new BlogDbContext())
            {
                var userId = User.Identity.GetUserId();
                var posts = database.TextPosts.Where(t => t.AuthorId == userId).ToList();
                int totalScore = 0;
                foreach (var post in posts)
                {
                    totalScore += post.Score;
                }
                return totalScore;
            }
        }
        //POST 
        [HttpPost]
        public ActionResult ChnageProfilePicture()
        {
            //This works only if you are logged in

            if (User.Identity.IsAuthenticated)
            {
                // To convert the user uploaded Photo as Byte Array before save to DB
                byte[] imageData = null;
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase poImgFile = Request.Files["UserPhoto"];

                    using (var binary = new BinaryReader(poImgFile.InputStream))
                    {
                        imageData = binary.ReadBytes(poImgFile.ContentLength);
                    }
                }

                var store = new UserStore<ApplicationUser>(new BlogDbContext());
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;


                var db = new BlogDbContext();
                var u =  db.Users.Find(user.Id);

                if (u != null)
                {
                    u.UserPhoto = imageData;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "UserProfile");

        }

    }
}
