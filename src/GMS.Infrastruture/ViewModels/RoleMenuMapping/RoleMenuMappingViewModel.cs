using GMS.Infrastructure.Models.EHRMS;
using GMS.Infrastructure.Models.RoleMenuMapping;

namespace GMS.Infrastructure.ViewModels.RoleMenuMapping
{
    public class RoleMenuMappingViewModel
    {
        public List<RoleMenuMappingDTO>? RoleMenuMappings { get; set; }
        public List<MenuListWithAttr>? MenuListWithAttrs { get; set; }
        public List<RoleMasterDTO>? Roles { get; set; }
    }
}
