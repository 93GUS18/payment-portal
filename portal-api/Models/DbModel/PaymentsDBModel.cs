using portal_api.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace portal_api.Models.DbModel
{
    [Table("Payments")]
    public class PaymentsDBModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public CurrencyEnum Currency { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 13, ErrorMessage = "Reference must be between 10 and 20 characters")]
        [AllowNull]
        public string Reference { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }        
        public bool IsDeleted { get; set; }
    }
}
