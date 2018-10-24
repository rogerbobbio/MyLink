using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLink.Models
{
    public class UserRol
    {
        [Key]
        public int UserRolId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "User Rol")]
        [Index("UserRol_Name_Index", IsUnique = true)]
        public string Name { get; set; }        
                
        public virtual ICollection<User> Users { get; set; }
    }
}