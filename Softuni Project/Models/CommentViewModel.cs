using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string AuthorName { get; set; }

        [Required]
        public int TextPostId { get; set; }

        [Required]
        public string Content { get; set; }

        public int Score { get; set; }

      
        public bool IsAuthor(string name)
        {
            return this.AuthorName.Equals(name);
        }
    }
}
