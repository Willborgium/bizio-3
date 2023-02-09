using System;
using System.Collections.Generic;

namespace Bizio.App.Game
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public ICollection<PersonSkill> Skills { get; set; } = new List<PersonSkill>();
    }
}
