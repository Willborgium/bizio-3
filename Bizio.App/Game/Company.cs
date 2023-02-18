using System;
using System.Collections.ObjectModel;

namespace Bizio.App.Game
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Employee Founder { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public bool IsPlayerCompany { get; set; }
        public float Money { get; set; }
        public ObservableCollection<Project> Projects { get; set; }

        public Company()
        {
            Employees = new();
            Projects = new();
        }
    }
}
