using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
    }
}