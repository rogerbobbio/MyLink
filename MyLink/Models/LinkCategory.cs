﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyLink.Models
{
    public class LinkCategory
    {
        [Key]
        public int LinkCategoryId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Categoria")]
        [Index("LinkCategory_Name_Index", IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<Link> Links { get; set; }
    }
}