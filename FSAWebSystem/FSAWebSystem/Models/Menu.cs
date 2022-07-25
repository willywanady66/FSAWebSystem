namespace FSAWebSystem.Models
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<RoleUnilever> RoleUnilevers { get; set; }
    }
}
