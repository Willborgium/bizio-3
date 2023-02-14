using System;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App.Game
{
    public class GameData
    {
        public Guid GameId { get; }
        public int Turn { get; set; }
        public ICollection<Person> People { get; }
        public ICollection<Company> Companies { get; }
        public Company PlayerCompany => Companies?.FirstOrDefault(c => c.IsPlayerCompany);
        public ICollection<Project> Projects { get; }

        public GameData()
        {
            GameId = Guid.NewGuid();
            Turn = 1;
            People = new List<Person>();
            Companies = new List<Company>();
            Projects = new List<Project>();
        }
    }
}
