namespace GMS.Infrastructure.Models.Masters
{
    public class PaymentMethodDTO
    {
        public int Id { get; set; }

        public string? PaymentMethodName { get; set; }
        public string? PaymentMethodCode { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
