using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyLink.Models
{
    public class Link
    {
        [Key]
        public int LinkId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(256, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Descripcion")]
        [Index("Link_Description_Index", IsUnique = true)]        
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(1000, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Url")]
        [Index("Link_Url_Index", IsUnique = true)]
        public string Url { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]        
        public int Ranking { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de creacion")]
        public DateTime CreationDate { get; set; }

        public bool Pending { get; set; }

        public bool Subtitle { get; set; }

        public bool Series { get; set; }

        public int Chapter { get; set; }

        public int TotalChapter { get; set; }

        public bool Old { get; set; }

        public bool Top { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Idioma")]
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Categoria")]
        public int LinkCategoryId { get; set; }


        public virtual Language Language { get; set; }
        public virtual LinkCategory LinkCategory { get; set; }
    }
}