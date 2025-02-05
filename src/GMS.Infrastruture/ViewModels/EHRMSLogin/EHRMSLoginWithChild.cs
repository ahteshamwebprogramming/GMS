namespace GMS.Infrastructure.ViewModels.EHRMSLogin;

public class EHRMSLoginWithChild
{
    public int LoginId { get; set; }

    public string WorkerCode { get; set; } = null!;

    public string? UserPassword { get; set; }

    public bool? IsActive { get; set; }

    public decimal? WorkerId { get; set; }
    

    public int? LogintCount { get; set; }
    public string? WorkerName { get; set; }
}
