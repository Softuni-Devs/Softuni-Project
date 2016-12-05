using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Softuni_Project.Models
{
    public class UserProfile
    {

        [Required]
        [Display(Name = "UserPhoto")]
        public byte[] UserPhoto { get; set; }


    }
}
