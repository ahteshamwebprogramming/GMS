using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters;

public class ResourceMasterViewModel
{
    public ResourceMasterDTO? ResourceMaster { get; set; }
    public List<ResourceMasterDTO>? ResourceMasters { get; set; }

    public List<ResourceMasterWithChild>? ResourceMasterWithChildren { get; set; }
    public List<RoleMasterDTO>? Roles { get; set; }
}
