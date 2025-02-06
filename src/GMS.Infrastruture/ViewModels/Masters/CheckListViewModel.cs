using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters
{
    public class CheckListViewModel
    {
        public string? CheckListType { get; set; }
        public TblCheckListsDTO? TblCheckLists { get; set; }
        public List<TblCheckListsDTO>? TblCheckListss { get; set; }
    }
}
