using System;
using System.Collections.Generic;

namespace Bizio.App.Game
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Value { get; set; }
        public int TurnDue { get; set; }
        public ICollection<ProjectRequirement> Requirements { get; }

        public Project()
        {
            Requirements = new List<ProjectRequirement>();
        }
    }
}
