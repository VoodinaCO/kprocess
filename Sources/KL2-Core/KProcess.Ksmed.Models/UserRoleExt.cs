namespace KProcess.Ksmed.Models
{
    public partial class UserRole
    {
        public int UserId { get; set; }
        public string RoleCode { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}
