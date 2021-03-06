using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class TextPost
    {
        private ICollection<Comment> comments;

        public TextPost()
        {
            Comments = new HashSet<Comment>();
            DatePosted = DateTime.Now;
            UsersLikesIDs = String.Empty;

        }
        public TextPost(string authorId, string title, string content, int categoryId)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Content = content;
            this.CategoryId = categoryId;
            this.comments = new HashSet<Comment>();
            UsersLikesIDs = String.Empty;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public int Score { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UsersLikesIDs { get; set; }

        public DateTime DatePosted { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Comment> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }

    }
}
