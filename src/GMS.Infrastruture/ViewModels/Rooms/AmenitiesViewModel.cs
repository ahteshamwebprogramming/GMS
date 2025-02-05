using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.Rooms;

namespace GMS.Infrastructure.ViewModels.Rooms
{
    public class AmenitiesViewModel
    {
        public List<AmenitiesDTO>? Amenitiess { get; set; }
        public List<AmenetiesCategoryDTO>? AmenetiesCategories { get; set; }
        public AmenitiesDTO? Amenities { get; set; }
    }
}
