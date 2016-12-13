using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Softuni_Project.Models
{
  

    public class BlogDbContext : IdentityDbContext<ApplicationUser>
    {
        public BlogDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }


        public virtual IDbSet<TextPost> TextPosts { get; set; }

        public virtual IDbSet<Comment> Comments { get; set; }
        
        public virtual IDbSet<Category> Categories { get; set; }

        public static BlogDbContext Create()
        {
            return new BlogDbContext();
        }
    }
}
