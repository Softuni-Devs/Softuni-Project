using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int TextPostId { get; set; }

        public virtual TextPost TextPost { get; set; }

        //Likes
        public int Score { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UsersLikesIDs { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }
        public Comment()
        {
           
            UsersLikesIDs = String.Empty;

        }
    }
}
