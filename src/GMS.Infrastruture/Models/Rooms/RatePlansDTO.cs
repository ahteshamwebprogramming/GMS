namespace GMS.Infrastructure.Models.Rooms;

public class RatePlansDTO
{
    public int Id { get; set; }
    public string? PlanName { get; set; }
    public string? PlanDetail { get; set; }
    public string? Inclusions { get; set; }
    public string? Exclusions { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}
