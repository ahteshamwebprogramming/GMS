using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;

[Dapper.Contrib.Extensions.Table("GuestReservation")]
public class GuestReservation
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Fname { get; set; }

    public string? Mname { get; set; }

    public string? Lname { get; set; }

    public string? CountryCode { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public int? SourceId { get; set; }

    public int? PackageId { get; set; }

    public string? NoOfNights { get; set; }

    public int? RtypeId { get; set; }

    public int? Pax { get; set; }

    public int? NoOfRooms { get; set; }

    public string? Doa { get; set; }

    public string? Dod { get; set; }

    public string? Remarks { get; set; }

    public int? EnqType { get; set; }

    public string? Reason { get; set; }

    public string? ReasonType { get; set; }

    public string? Comments { get; set; }

    public string? AltContactNo { get; set; }

    public string? PrefTime { get; set; }

    public string? Dob { get; set; }

    public string? Address { get; set; }

    public int? City { get; set; }

    public string? Country { get; set; }

    public string? Profession { get; set; }

    public string? BrandAwn { get; set; }

    public int? IsPick { get; set; }

    public string? PickLocation { get; set; }

    public string? ArrDate { get; set; }

    public string? DepDate { get; set; }
    public DateTime? DateOfArrival { get; set; }
    public DateTime? DateOfDepartment { get; set; }
    public int? RoomType { get; set; }
    public int? SaleSource { get; set; }

    public string? CarType { get; set; }

    public DateTime? CrDate { get; set; }

    public int? Ccode { get; set; }

    public int? Gcode { get; set; }

    public int? BaawId { get; set; }

    public int? Gender { get; set; }

    public string? HoldTill { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentDate { get; set; }

    public int? Status { get; set; }

    public string? Passport { get; set; }

    public string? RefNo { get; set; }

    public int? UserId { get; set; }

    public int? CatId { get; set; }
    public int? ServiceId { get; set; }

    public int? CallStatus { get; set; }

    public int? AlternateEmail { get; set; }

    public int? ScheduleType { get; set; }

    public int? LeadSource { get; set; }

    public string? Nationality { get; set; }

    public int? State { get; set; }

    public int? PinCode { get; set; }

    public int? AdditionalNights { get; set; }

    public int? PickUpType { get; set; }
}
