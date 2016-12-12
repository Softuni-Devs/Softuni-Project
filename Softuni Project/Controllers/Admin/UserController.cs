using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Softuni_Project.Models;

namespace Softuni_Project.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        //GET: User/List

        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var users = db.Users.ToList();

                var admins = GetAdminUserNames(users, db);
                ViewBag.Admins = admins;


                return View(users);
            }

        }

        private HashSet<string> GetAdminUserNames(List<ApplicationUser> users, BlogDbContext context)
        {

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var admins = new HashSet<string>();

            foreach (var user in users)
            {

                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.UserName);
                }


            }
         
            return admins;

        }


        //GET: User/Edit
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var user = db.Users.Where(u=>u.Id==id).First();

                if (user == null)
                {
                    return HttpNotFound();
                }

                //create a view model
                var viewModel = new EditUserViewModel();
                viewModel.User = user;
                viewModel.Roles = GetUserRoles(user, db);


                return View(viewModel);

            }
            
        }

        private IList<Role> GetUserRoles(ApplicationUser user, BlogDbContext db)
        {
           var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //get all application roles
            var roles = db.Roles.Select(r => r.Name).OrderBy(r => r).ToList();

            //for each application role check if the user has it

            var userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                

                var role = new Role {Name=roleName};

                if (userManager.IsInRole(user.Id, roleName))
                {

                    role.IsSelected = true;
                }
                userRoles.Add(role);
            }
            return userRoles;

        }


        //POST: User/Edit
        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {

                    var user = db.Users.FirstOrDefault(u => u.Id == id);

                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    //if the password field is not empty, change the password

                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        var hasher = new PasswordHasher();
                        var passwordHash = hasher.HashPassword(viewModel.Password);
                        user.PasswordHash = passwordHash;

                    }
                    user.Email = viewModel.User.Email;
                    user.FullName = viewModel.User.FullName;
                    user.UserName = viewModel.User.Email;
                    this.setUserRoles(viewModel,user,db);

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("List");
                }



            }
            return View(viewModel);



        }

        private void setUserRoles(EditUserViewModel viewModel, ApplicationUser user, BlogDbContext context)
        {
           
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            foreach (var role in viewModel.Roles)
            {

                if (role.IsSelected && !userManager.IsInRole(user.Id, role.Name))
                {
                    userManager.AddToRole(user.Id,role.Name);
                }
                else if (!role.IsSelected && userManager.IsInRole(user.Id, role.Name))
                {
                    userManager.RemoveFromRole(user.Id,role.Name);
                }


            }


        }


        //GET: User/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                
                var user = db.Users.Where(u=>u.Id.Equals(id)).First();

                if (user == null)
                {
                    return HttpNotFound();
                }


                return View(user);
            }

        }

        //POST: User/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {

                var user = db.Users.Where(u => u.Id.Equals(id)).First();

                var userPosts = db.TextPosts.Where(a => a.AuthorId.Equals(user.Id));

                var userComments = db.Comments.Where(c => c.AuthorId.Equals(user.Id));

                foreach (var post in userPosts)
                {
                    db.TextPosts.Remove(post);
                }
                foreach (var comment in userComments)
                {
                    db.Comments.Remove(comment);
                }

                db.Users.Remove(user);
                db.SaveChanges();

                return RedirectToAction("List");
            }

        }

    }
}
