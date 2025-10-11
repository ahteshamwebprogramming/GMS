using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("GuestCancellationVerification")]
public class GuestCancellationVerification
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int GuestId { get; set; }

    public decimal CancellationAmount { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedOn { get; set; }

    public int? VerifiedBy { get; set; }
}
