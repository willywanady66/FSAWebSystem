namespace FSAWebSystem.Models
{
    public class RoleAccess
    {
        public Guid Id { get; set; }
        public RoleUnilever Role { get; set; }
        public string Menu { get; set; }
        public string SubMenu { get; set; }
        public string AccessActivity { get; set; }
    }
}
