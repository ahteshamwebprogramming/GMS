namespace GMS.Infrastructure.Models.Guests
{
    public class GuestScheduleDTO
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int? TaskId { get; set; }
        public int? EmployeeId1 { get; set; }
        public int? EmployeeId2 { get; set; }
        public int? SessionId { get; set; }
        public int? ResourceId { get; set; }
    }
}
