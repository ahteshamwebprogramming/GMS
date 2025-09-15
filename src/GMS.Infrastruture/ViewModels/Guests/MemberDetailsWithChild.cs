using GMS.Infrastructure.Models.Guests;

namespace GMS.Infrastructure.ViewModels.Guests;

public class MemberDetailsWithChild : MembersDetailsDTO
{

    public int? IsChecked { get; set; }
    public string? GenderName { get; set; }
    public int? GuestIdPaxSN1 { get; set; }
    public string? Service { get; set; }
    public string? Category { get; set; }

    public string? RoomTypeName { get; set; }
    //public int? PaxSNo { get; set; }
    public int? NoOfNights { get; set; }
    public string? RType { get; set; }
    public bool InHouse { get; set; }
    public int? CheckInStatus { get; set; }
    public double? PackagePrice { get; set; }
    public double? RoomPrice { get; set; }
    public string? GuestTask { get; set; }
    public DateTime? ActivityStartTime { get; set; }
    public DateTime? ActivityEndTime { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? ActDateOfArrival { get; set; }
    public DateTime? ActDateOfDepartment { get; set; }
    public bool? IsSettled { get; set; }
}
