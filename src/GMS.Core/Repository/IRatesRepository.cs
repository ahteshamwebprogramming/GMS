using GMS.Core.Entities;
using GMS.Infrastructure.Models.Rooms;
using GMS.Infrastructure.ViewModels.Rooms;
namespace GMS.Core.Repository;

public interface IRatesRepository : IDapperRepository<Rates>
{
    Task<bool> UpdateRatesAsync(List<RatesDTO> rates);

    Task UpdateBulkInventoryAsync(BulkUpdateViewModel model);
    Task UpdateBulkRatesAsync(int channelId, DateTime fromDate, DateTime toDate, List<RoomRateBulkUpdate> rates);
    Task UpdateBulkRatesAsyncPackages(int channelId, DateTime fromDate, DateTime toDate, List<RoomRateBulkUpdate> rates,string selectedDaysList);

    Task SaveRoomPercentagesAsync(List<RoomPercentageData> percentageData, DateTime fromDate, DateTime toDate);

    Task UpdateBulkRestrictionsAsync(int channelId, DateTime fromDate, DateTime toDate, List<RoomRestrictionBulkUpdate> restrictions);
}
