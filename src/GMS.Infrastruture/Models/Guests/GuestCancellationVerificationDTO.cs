using System;

namespace GMS.Infrastructure.Models.Guests;

public class GuestCancellationVerificationDTO
{
    public int Id { get; set; }

    public int GuestId { get; set; }

    public decimal CancellationAmount { get; set; }

    public decimal? TotalAmountPaid { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedOn { get; set; }

    public int? VerifiedBy { get; set; }
}
