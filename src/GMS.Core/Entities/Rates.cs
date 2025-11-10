using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("Rates")]
    public class Rates
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public int RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal MinRate { get; set; }
        public decimal MaxRate { get; set; }
        public string? OTAID { get; set; }
        public decimal? PlanId { get; set; }
        public int? CancellationDays { get; set; }
        public bool? StopSell { get; set; }
        public bool? CloseOnArrival { get; set; }
        public bool? RestrictStay { get; set; }
        public int? MinimumNights { get; set; }
        public int? MaximumNights { get; set; }
    }
}
