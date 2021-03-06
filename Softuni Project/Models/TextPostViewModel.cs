using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class TextPostViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [DisplayName("Post")]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        [ForeignKey("Category")]
        [DisplayName("Category")]
        [Required]
        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; }

        public int Score { get; set; }

       

      


    }
}
