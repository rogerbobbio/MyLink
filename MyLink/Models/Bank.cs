using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLink.Models
{
    public class Bank
    {
        [Key]
        public int BankId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Banco")]
        [Index("Bank_Name_Index", IsUnique = true)]
        public string Name { get; set; }
    }
}