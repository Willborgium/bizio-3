using System;
using System.Collections.ObjectModel;

namespace Bizio.App.Game
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public Gender Gender { get; set; }
        public ObservableCollection<PersonSkill> Skills { get; set; } = new();
    }
}
