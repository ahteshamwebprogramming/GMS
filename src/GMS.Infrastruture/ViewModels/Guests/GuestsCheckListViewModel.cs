using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Guests;

public class GuestsCheckListViewModel
{
    public MemberDetailsWithChild? MemberDetails { get; set; }
    public List<TblCheckListsDTO>? CheckListOut { get; set; }
    public List<TblCheckListsDTO>? CheckListIn { get; set; }
    public List<TblCheckListsDTO>? CheckList { get; set; }
}
