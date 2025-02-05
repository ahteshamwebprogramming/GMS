namespace GMS.Infrastructure.ViewModels.Guests;

public class GuestsGridViewParameters
{
    public string? GuestsListType { get; set; }
    public string? SearchKeyword { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public int? TotalPages { get; set; }
    public int? TotalRecords { get; set; }
    public string? Source { get; set; }
}
