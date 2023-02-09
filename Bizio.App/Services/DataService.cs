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

            return new Company
            {
                Id = Guid.NewGuid(),
                Name = StaticData.CompanyNames.Random(),
                Founder = founderEmployee,
                Employees = new List<Employee>
                {
                    founderEmployee
                }
            };
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
