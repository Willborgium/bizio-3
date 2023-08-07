namespace Hyjynx.Bizio.Game
{
    public enum ProjectStatus
    {
        InProgress,
        Completed,
        Failed
    }

    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectStatus Status { get; set; }
        public float Value { get; set; }
        public int TurnDue { get; set; }
        public ICollection<ProjectRequirement> Requirements { get; }

        public Project()
        {
            Requirements = new List<ProjectRequirement>();
        }
    }
}
