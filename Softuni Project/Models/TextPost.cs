using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class TextPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }


        //Likes / Dislikes 
        public int Score { get; set; }

        public DateTime DatePosted { get; set; }


        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UsersLikesIDs { get; set; }



        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }



        public TextPost()
        {

            Comments = new HashSet<Comment>();
            DatePosted = DateTime.Now;
            UsersLikesIDs = String.Empty;
            
        }

    }
}
