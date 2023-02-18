using Bizio.App.Game;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bizio.App.Services
{
    public class DataService
    {
        public StaticData StaticData { get; private set; }

        public GameData CurrentGame { get; private set; }

        public void Initialize()
        {
            StaticData = LoadDataFromFile<StaticData>(StaticDataFilePath);
        }

        public void InitializeNewGame()
        {
            var gameData = new GameData();

            var personBatchSize = StaticData.NewGamePeopleBatchSize.Random();

            for (var personIndex = 0; personIndex < personBatchSize; personIndex++)
            {
                var person = GeneratePerson();

                gameData.People.Add(person);
            }

            var companyBatchSize = StaticData.NewGameCompanyBatchSize.Random();

            var founderIds = new List<Guid>();

            for (var companyIndex = 0;companyIndex < companyBatchSize; companyIndex++)
            {
                var company = GenerateCompany(gameData.People, founderIds);

                if (gameData.Companies.Count == 0)
                {
                    company.IsPlayerCompany = true;
                }

                founderIds.Add(company.Founder.PersonId);

                gameData.Companies.Add(company);
            }

            var projectBatchSize = StaticData.NewGameProjectBatchSize.Random();
            
            for (var projectIndex = 0; projectIndex < projectBatchSize; projectIndex++)
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = $"Project {projectIndex + 1}",
                    Description = $"Description on project {projectIndex + 1}",
                    TurnDue = gameData.Turn + _r.Next(5, 10),
                    Value = (float)Math.Round(_r.NextDouble() + .1, 1) * 500f                    
                };

                var requirementCount = _r.Next(1, StaticData.Skills.Count);

                for (var requirementIndex = 0; requirementIndex < requirementCount; requirementIndex++)
                {
                    var requirement = new ProjectRequirement
                    {
                        SkillId = StaticData.Skills.Where(s => !project.Requirements.Any(r => r.SkillId == s.Id)).Random().Id,
                        TargetAmount = (float)Math.Round(_r.NextDouble() + .1, 1) * 100f
                    };

                    project.Requirements.Add(requirement);
                }

                gameData.Projects.Add(project);
            }

            CurrentGame = gameData;
        }

        private Person GeneratePerson()
        {
            var gender = _r.Random<Gender>();

            var firstName = StaticData.FirstNames[gender].Random();

            var lastName = StaticData.LastNames.Random();

            var person = new Person
            {
                Id = Guid.NewGuid(),
                Gender = gender,
                FirstName = firstName,
                LastName = lastName
            };

            var skillCount = StaticData.NewPersonSkillCount.Random();

            for (var skillIndex = 0; skillIndex < skillCount; skillIndex++)
            {
                var skill = StaticData.Skills.Random(s => !person.Skills.Any(ps => ps.SkillId == s.Id));

                var personSkill = new PersonSkill
                {
                    SkillId = skill.Id,
                    LearnRate = _r.Random<PersonSkillLearnRate>(),
                    Value = _r.Next(1, 100)
                };

                person.Skills.Add(personSkill);
            }

            return person;
        }

        private Company GenerateCompany(IEnumerable<Person> people, IEnumerable<Guid> founderIds)
        {
            var founder = people.Random(p => !founderIds.Contains(p.Id));

            var founderEmployee = new Employee
            {
                PersonId = founder.Id,
                Salary = 0
            };

            var company =  new Company
            {
                Id = Guid.NewGuid(),
                Name = StaticData.CompanyNames.Random(),
                Money = 1500,
                Founder = founderEmployee
            };

            company.Employees.Add(founderEmployee);

            return company;
        }

        private static T LoadDataFromFile<T>(string path)
        {
            string data;

            using (var reader = new StreamReader(path))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        private const string StaticDataFilePath = "Content/StaticData.json";

        private static readonly Random _r = new();
    }
}
