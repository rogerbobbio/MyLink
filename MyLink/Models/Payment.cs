using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MyLink.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Proyecto")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Cuenta")]
        public string Account { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Codigo Interbancario")]
        public string InterbankCode { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Banco")]
        public string Bank { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The field {0} must be at least {1} characteres length.")]
        [Display(Name = "Persona a Cargo")]
        public string PersonCharge { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(0, double.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Voucher { get; set; }

        [NotMapped]
        [Display(Name = "Comprobante")]
        public HttpPostedFileBase PaymentFile { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de operacion")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Pago Proveedor")]
        public bool ProviderFlag { get; set; }

        public virtual Project Project { get; set; }
    }
}