using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("MembersDetails")]
public class MembersDetails
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Fname { get; set; }

    public string? Mname { get; set; }

    public string? Lname { get; set; }

    public string? PhNo { get; set; }

    public string? CountryCode { get; set; }

    public string? MobileNo { get; set; }

    public string? EmergencyNo { get; set; }

    public string? Dob { get; set; }

    public string? MarStatus { get; set; }

    public int? Gender { get; set; }

    public string? PhysicianName { get; set; }

    public string? PhysicianNumber { get; set; }

    public string? ReferredBy { get; set; }

    public string? ServiceId { get; set; }

    public string? LegalName { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Pincode { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? Email { get; set; }

    public string? Idproof { get; set; }

    public string? IsMonthlynewsletter { get; set; }

    public string? AboutUs { get; set; }

    public string? RelativeName { get; set; }

    public string? Relations { get; set; }

    public string? RelativeNumber { get; set; }

    public string? ReferContact { get; set; }

    public string? UniqueNo { get; set; }

    public int? Status { get; set; }

    public int? IsApproved { get; set; }

    public string? Remarks { get; set; }

    public int? UserId { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? AprovedDate { get; set; }

    public string? Photo { get; set; }

    public string? BloodGroup { get; set; }

    public string? Occupation { get; set; }

    public string? PassportNo { get; set; }

    public string? VisaDetails { get; set; }

    public string? PolicyHolder { get; set; }

    public string? InsuranceCompany { get; set; }

    public string? PolicyNo { get; set; }

    public string? CoveredIndia { get; set; }

    public int? Staydays { get; set; }

    public int? RoomType { get; set; }

    public string? HealthIssues { get; set; }

    public int? CatId { get; set; }

    public int? IsCrm { get; set; }

    public string? Nights { get; set; }

    public string? Nationality { get; set; }

    public int? GuarenteeCode { get; set; }

    public int? PaymentStatus { get; set; }

    public DateTime? HoldTillDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? LeadSource { get; set; }

    public int? ChannelCode { get; set; }

    public int? AdditionalNights { get; set; }

    public DateTime? DateOfArrival { get; set; }

    public DateTime? DateOfDepartment { get; set; }

    public int? Pax { get; set; }

    public int? NoOfRooms { get; set; }

    public int? PickUpDrop { get; set; }

    public int? PickUpType { get; set; }

    public int? CarType { get; set; }

    public string? FlightArrivalDateAndDateTime { get; set; }

    public string? FlightDepartureDateAndDateTime { get; set; }

    public string? PickUpLoaction { get; set; }

    public int? Age { get; set; }

    [Computed]
    public string CustomerName { get; set; }
    public string GroupId { get; set; }
    public int PAXSno { get; set; }
    public bool PaxCompleted { get; set; }
}
