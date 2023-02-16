using System;
using System.Collections.Generic;

namespace Bizio.App.Game
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Employee Founder { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public bool IsPlayerCompany { get; set; }
        public float Money { get; set; }
        public ICollection<Project> Projects { get; set; }

        public Company()
        {
            Employees = new List<Employee>();
            Projects = new List<Project>();
        }
    }
}
