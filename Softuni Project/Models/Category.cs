using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class Category
    {
        private ICollection<TextPost> textposts;
        public Category()
        {
            this.textposts = new HashSet<TextPost>();
        }


        [Key]
        public int Id { get; set;}

        [Required]
        [Index(IsUnique =true)]
        [StringLength(20)]
        public string Name { get; set; }

        public virtual ICollection<TextPost> TextPosts { get; set; }

    }
}