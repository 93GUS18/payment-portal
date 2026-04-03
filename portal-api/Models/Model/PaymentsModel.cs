using portal_api.Models.Enum;

namespace portal_api.Models.Model
{
    public class PaymentsModel
    {
        public int Amount { get; set; }
        public CurrencyEnum Currency { get; set; }
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
