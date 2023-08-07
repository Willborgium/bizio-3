using Hyjynx.Bizio.Game;
using Hyjynx.Core;
using Newtonsoft.Json;

namespace Hyjynx.Bizio.Services
{
    public class DataService : IDataService
    {
        public StaticData StaticData { get; private set; }

        public GameData CurrentGame { get; private set; }

        public void Initialize()
        {
            StaticData = LoadDataFromFile<StaticData>(StaticDataFilePath);
        }

        public void InitializeNewGame(Action<Company> playerCompanyInitializeHook)
        {
            var gameData = new GameData();

            var personBatchSize = StaticData.NewGamePeopleBatchSize.Random();

            for (var personIndex = 0; personIndex < personBatchSize; personIndex++)
            {
                gameData.People.Add(GeneratePerson());
            }

            var companyBatchSize = StaticData.NewGameCompanyBatchSize.Random();

            for (var companyIndex = 0; companyIndex < companyBatchSize; companyIndex++)
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

            while (projectBatchSize > 0)
            {
                gameData.Projects.Add(GenerateProject(gameData.Turn));
                projectBatchSize--;
            }

            CurrentGame = gameData;
        }

        public void ProcessTurn()
        {
            if (CurrentGame.Turn % 5 == 0)
            {
                while (CurrentGame.People.Count < 10)
                {
                    CurrentGame.People.Add(GeneratePerson());
                }

                var projectBatchSize = StaticData.NewGameProjectBatchSize.Random();

                while (projectBatchSize > 0)
                {
                    CurrentGame.Projects.Add(GenerateProject(CurrentGame.Turn));
                    projectBatchSize--;
                }
            }
        }

        public void SendMessage(string from, string subject, string body)
        {
            CurrentGame.PlayerCompany.Messages.Add(new Message
            {
                TurnSent = CurrentGame.Turn,
                From = from,
                To = $"ceo@{CurrentGame.PlayerCompany.Name.ToLower()}.com",
                Subject = subject,
                Body = body
            });
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

        private Project GenerateProject(int currentTurn)
        {
            var id = Guid.NewGuid();
            var idString = id.ToString()[30..];

            var project = new Project
            {
                Id = id,
                Name = $"Project {idString}",
                Description = $"Description of project {idString}",
                TurnDue = currentTurn + _r.Next(5, 10),
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

            return project;
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
