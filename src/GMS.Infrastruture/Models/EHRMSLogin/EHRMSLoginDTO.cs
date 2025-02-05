namespace GMS.Infrastructure.Models.EHRMSLogin;

public class EHRMSLoginDTO
{
    public int LoginId { get; set; }

    public string WorkerCode { get; set; } = null!;

    public string? UserPassword { get; set; }

    public bool? IsActive { get; set; }

    public decimal? WorkerId { get; set; }

    public int? LogintCount { get; set; }
}
