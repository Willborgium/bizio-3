using Bizio.App.Game;
using Newtonsoft.Json;
using System;
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

        public void InitializeNewGame(Action<GameData> globalInitializeHook, Action<Company> playerCompanyInitializeHook)
        {
            var gameData = new GameData();

            globalInitializeHook?.Invoke(gameData);

            var personBatchSize = StaticData.NewGamePeopleBatchSize.Random();

            for (var personIndex = 0; personIndex < personBatchSize; personIndex++)
            {
                var person = GeneratePerson();

                gameData.People.Add(person);
            }

            var companyBatchSize = StaticData.NewGameCompanyBatchSize.Random();

            for (var companyIndex = 0;companyIndex < companyBatchSize; companyIndex++)
            {
                var company = new Company();

                if (gameData.Companies.Count == 0)
                {
                    company.IsPlayerCompany = true;
                    playerCompanyInitializeHook?.Invoke(company);
                }

                InitializeCompany(company, gameData.People);

                gameData.People.Remove(company.Founder.Person);
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

        private void InitializeCompany(Company company, IEnumerable<Person> people)
        {
            var founder = people.Random();

            var founderEmployee = new Employee
            {
                Person = founder,
                IsFounder = true,
                Salary = 0
            };

            company.Id = Guid.NewGuid();
            company.Name = StaticData.CompanyNames.Random();
            company.Money = 1500;

            company.Employees.Add(founderEmployee);
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
