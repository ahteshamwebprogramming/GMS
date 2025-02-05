namespace GMS.Infrastructure.Models.Guests
{
    public class GuestScheduleDTO
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeOnly Duration { get; set; }
        public int? TaskId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ResourceId { get; set; }
    }
}
