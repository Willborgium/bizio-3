using System.Collections.ObjectModel;

namespace Hyjynx.Bizio.Game
{
    public class GameData
    {
        public Guid GameId { get; }
        public int Turn { get; set; }
        public ObservableCollection<Person> People { get; }
        public ICollection<Company> Companies { get; }
        public Company PlayerCompany => Companies?.FirstOrDefault(c => c.IsPlayerCompany);
        public ObservableCollection<Project> Projects { get; }

        public GameData()
        {
            GameId = Guid.NewGuid();
            Turn = 1;
            People = new();
            Companies = new List<Company>();
            Projects = new();
        }
    }
}
