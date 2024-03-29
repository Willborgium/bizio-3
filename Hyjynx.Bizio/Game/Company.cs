﻿using System.Collections.ObjectModel;

namespace Hyjynx.Bizio.Game
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public bool IsPlayerCompany { get; set; }
        public float Money { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<Project> ArchivedProjects { get; set; }
        public Employee Founder => Employees?.FirstOrDefault(e => e.IsFounder);
        public ObservableCollection<Allocation> Allocations { get; set; }
        public ObservableCollection<Message> Messages { get; set; }

        public Company()
        {
            Employees = new();
            Projects = new();
            ArchivedProjects = new();
            Allocations = new();
            Messages = new();
        }
    }
}
