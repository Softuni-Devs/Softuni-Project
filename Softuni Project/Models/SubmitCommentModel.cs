using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class SubmitCommentModel
    {

        [Required]
        public string Comment { get; set; }

        [Required]
        public int TextPostId { get; set; }
    }
}
