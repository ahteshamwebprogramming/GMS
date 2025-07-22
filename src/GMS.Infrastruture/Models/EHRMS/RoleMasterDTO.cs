namespace GMS.Infrastructure.Models.EHRMS
{
    public class RoleMasterDTO
    {
        public int Id { get; set; }
        public string? Role { get; set; }

        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public char? isDefault { get; set; }
        public char? isActive { get; set; }
    }
}
