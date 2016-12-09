using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Softuni_Project.Models;

namespace Softuni_Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ListAll", "TextPost");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        public FileContentResult DisplayProfilePicture()
        {

            if (User.Identity.IsAuthenticated)
            {
                var store = new UserStore<ApplicationUser>(new BlogDbContext());
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

                var db = new BlogDbContext();
                var u = db.Users.Find(user.Id);

                var userImage = u.UserPhoto;


                if (userImage == null)
                {
                    CreateDefaultProfilePic();
                }
                else
                {

                    return new FileContentResult(userImage, "image/jpeg");

                }
            }

            
            // Create a new default profile picture if the user is not logged in.
           return CreateDefaultProfilePic();

        }


        public FileContentResult CreateDefaultProfilePic()
        {
            string fileName = HttpContext.Server.MapPath(@"~/Images/nopic.jpg");

            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(fileName);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);

            return File(imageData, "image/png");
        }

    }
}
