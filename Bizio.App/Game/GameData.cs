using System;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App.Game
{
    public class GameData
    {
        public Guid GameId { get; }
        public ICollection<Person> People { get; }
        public ICollection<Company> Companies { get; }
        public Company PlayerCompany => Companies?.FirstOrDefault(c => c.IsPlayerCompany);

        public GameData()
        {
            GameId = Guid.NewGuid();
            People = new List<Person>();
            Companies = new List<Company>();
        }
    }
}
