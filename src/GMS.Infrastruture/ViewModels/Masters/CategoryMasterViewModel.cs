using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters;

public class CategoryMasterViewModel
{
    public CategoryMasterDTO? CategoryMaster { get; set; }
    public List<CategoryMasterDTO>? CategoryMasters { get; set; }
    public List<CategoryMasterWithAttributes>? CategoryMasterWithAttributes { get; set; }
    public List<RoleMasterDTO>? Roles { get; set; }
}
