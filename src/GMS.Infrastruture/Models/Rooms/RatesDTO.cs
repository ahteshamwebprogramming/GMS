namespace GMS.Infrastructure.Models.Rooms;

public class RatesDTO
{
    public int Id { get; set; }
    // public int HotelId { get; set; }
    public int RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public decimal MinRate { get; set; }
    public decimal MaxRate { get; set; }
    public string? OTAID { get; set; }
    public int? PlanId { get; set; }
}
