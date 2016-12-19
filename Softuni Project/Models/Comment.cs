using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class Comment
    {
        public Comment()
        {

            UsersLikesIDs = String.Empty;


        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public virtual TextPost TextPost { get; set; }

        public int TextPostId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public string AuthorId { get; set; }


        public int Score { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UsersLikesIDs { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }
    }
}
