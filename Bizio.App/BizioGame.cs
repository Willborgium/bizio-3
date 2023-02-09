using Bizio.App.Game;
using Bizio.App.Services;
using Bizio.App.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App
{
    public class BizioGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly ResourceService _resources = new();
        private readonly DataService _dataService = new();
        private readonly LoggingService _logger = new();

        private readonly ConcurrentBag<IUpdateable> _updateables;
        private readonly ICollection<IRenderable> _renderables;

        public BizioGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _updateables = new ConcurrentBag<IUpdateable>();
            _renderables = new List<IRenderable>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var font = Content.Load<SpriteFont>("font-default");
            _resources.Set("font-default", font);

            var pixel = Content.Load<Texture2D>("pixel");
            _resources.Set("texture-pixel", pixel);

            _logger.Initialize(font, pixel);
            _renderables.Add(_logger);

            var buttonSpritesheet = Content.Load<Texture2D>("greySheet");
            var buttonMetadata = new ButtonMetadata
            {
                Font = font,
                Spritesheet = buttonSpritesheet,
                DefaultSource = new Rectangle(0, 143, 190, 45),
                HoveredSource = new Rectangle(0, 98, 190, 45),
                ClickedSource = new Rectangle(0, 188, 190, 49),
                DisabledSource = new Rectangle(0, 0, 195, 49)
            };

            _resources.Set("button-spritesheet-default", buttonSpritesheet);
            _resources.Set("button-metadata-default", buttonMetadata);

            CreateButton("Logger", 1500, 10, 300, 50, ToggleDebugInfo);
            CreateButton("Snapshot", 1500, 70, 300, 50, LogSnapshot);
            CreateButton("New Game", 1500, 130, 300, 50, StartNewGame);
            CreateButton("People", 1500, 190, 300, 50, TogglePeopleList);
            CreateButton("My Company", 1500, 250, 300, 50, ToggleMyCompany);

            CreatePersonDetailsContainer();

            var peopleContainer = new Container();
            _resources.Set("container-people", peopleContainer);
            _renderables.Add(peopleContainer);

            var myCompanyContainer = new Container();
            _resources.Set("container-my-company", myCompanyContainer);
            _renderables.Add(myCompanyContainer);

            _dataService.Initialize();

            _logger.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            _logger.Info("Initialization complete");
        }

        private void LogSnapshot(object sender, EventArgs e)
        {
            var renderableCount = _renderables.Count;
            var updateableCount = _updateables.Count;

            _logger.Info("[BEGIN SNAPSHOT]");
            _logger.Info($"Renderables: {renderableCount}");
            _logger.Info($"Updateables: {updateableCount}");
            _logger.Info("[END SNAPSHOT]");
        }

        private void ToggleMyCompany(object sender, EventArgs e)
        {
            // show my company
        }

        private void CreatePersonDetailsContainer()
        {
            var root = new Container();
            _resources.Set("container-person-details", root);
            _renderables.Add(root);

            var font = _resources.Get<SpriteFont>("font-default");

            // Name
            var nameLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(500, 100),
                Text = "Name:"
            };

            _resources.Set("container-person-details-label-name", nameLabel);
            root.AddChild(nameLabel);

            var nameTextBox = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(600, 100)
            };

            _resources.Set("container-person-details-textbox-name", nameTextBox);
            root.AddChild(nameTextBox);

            // Gender
            var genderLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(500, 150),
                Text = "Gender:"
            };

            _resources.Set("container-person-details-label-gender", genderLabel);
            root.AddChild(genderLabel);

            var genderTextBox = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(600, 150)
            };

            _resources.Set("container-person-details-textbox-gender", genderTextBox);
            root.AddChild(genderTextBox);

            // Hire Button
            var hireButton = CreateButton("Hire", 500, 200, 300, 50, HireCurrentPerson);
            _resources.Set("container-person-details-button-hire", hireButton);
            _renderables.Remove(hireButton);
            root.AddChild(hireButton);


            // Skills
            var skillsLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(500, 275),
                Text = "Skills"
            };

            _resources.Set("container-person-details-label-skills", skillsLabel);
            root.AddChild(skillsLabel);
        }

        private void HireCurrentPerson(object sender, EventArgs e)
        {
            var person = _resources.Get<Person>("container-person-details-current-person");

            if (person == null)
            {
                _logger.Warning("Somehow got to HireCurrentPerson without a current person!");
                return;
            }

            var employee = new Employee
            {
                PersonId = person.Id,
                Salary = new Random().Next(1, 100)
            };

            _dataService.CurrentGame.PlayerCompany.Employees.Add(employee);

            var hireButton = _resources.Get<Button>("container-person-details-button-hire");
            hireButton.IsEnabled = false;
        }

        private void ToggleDebugInfo(object sender, EventArgs e)
        {
            _logger.IsVisible = !_logger.IsVisible;
            _logger.Info($"Logger toggled: {_logger.IsVisible}");
        }

        private void TogglePeopleList(object sender, EventArgs e)
        {
            var container = _resources.Get<Container>("container-people");
            container.IsVisible = !container.IsVisible;
            _logger.Info($"People container toggled: {container.IsVisible}");
        }

        private void StartNewGame(object sender, EventArgs e)
        {
            _dataService.InitializeNewGame();

            _logger.Info($"Game ID: {_dataService.CurrentGame.GameId}");
            _logger.Info($"Person count: {_dataService.CurrentGame.People.Count}");

            var offset = 10;

            var peopleContainer = _resources.Get<Container>("container-people");

            foreach (var person in _dataService.CurrentGame.People)
            {
                _logger.Info($"Person {person.Id}: {person.FirstName} {person.LastName}, Skills: {person.Skills.Count}");

                var button = CreateButton($"{person.FirstName} {person.LastName}", 10, offset, 300, 50, (s, e) => TogglePerson(person));
                _renderables.Remove(button);
                peopleContainer.AddChild(button);

                offset += 60;
            }

            _logger.Info($"Company count: {_dataService.CurrentGame.Companies.Count}");

            foreach (var company in _dataService.CurrentGame.Companies)
            {
                var founder = _dataService.CurrentGame.People.FirstOrDefault(p => p.Id == company.Founder.PersonId);
                _logger.Info($"Company {company.Id}: {company.Name} founded by {founder.FirstName} {founder.LastName}");
            }
        }

        private void TogglePerson(Person person)
        {
            var container = _resources.Get<Container>("container-person-details");

            if (person == null)
            {
                container.IsVisible = false;
                return;
            }

            var previousPerson = _resources.Get<Person>("container-person-details-current-person");

            if (previousPerson != null)
            {
                foreach (var skill in previousPerson.Skills)
                {
                    var skillLabel = _resources.Get<TextBox>($"container-person-details-label-skill-{skill.SkillId}");
                    container.RemoveChild(skillLabel);
                    _renderables.Remove(skillLabel);

                    var skillTextBox = _resources.Get<TextBox>($"container-person-details-textbox-skill-{skill.SkillId}");
                    container.RemoveChild(skillTextBox);
                    _renderables.Remove(skillTextBox);
                }
            }

            if (previousPerson == person)
            {
                _resources.Set("container-person-details-current-person", null);
                container.IsVisible = false;
                return;
            }

            var nameTextBox = _resources.Get<TextBox>("container-person-details-textbox-name");
            nameTextBox.Text = $"{person.FirstName} {person.LastName}";

            var genderTextBox = _resources.Get<TextBox>("container-person-details-textbox-gender");
            genderTextBox.Text = $"{person.Gender}";

            var hireButton = _resources.Get<Button>("container-person-details-button-hire");

            if (_dataService.CurrentGame.PlayerCompany.Employees.Any(e => e.PersonId == person.Id))
            {
                hireButton.IsEnabled = false;
            }
            else
            {
                hireButton.IsEnabled = true;
            }

            var offset = 300f;

            var font = _resources.Get<SpriteFont>("font-default");

            foreach (var personSkill in person.Skills.OrderByDescending(s => s.Value))
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == personSkill.SkillId);

                var skillLabel = new TextBox
                {
                    Font = font,
                    Color = Color.Black,
                    Position = new Vector2(550, offset),
                    Text = $"{skill.Name}",
                    IsVisible = true
                };

                container.AddChild(skillLabel);
                _resources.Set($"container-person-details-label-skill-{skill.Id}", skillLabel);

                var skillTextBox = new TextBox
                {
                    Font = font,
                    Color = Color.Black,
                    Position = new Vector2(650, offset),
                    Text = $"{personSkill.Value:0.0}",
                    IsVisible = true
                };

                offset += 30;

                container.AddChild(skillTextBox);
                _resources.Set($"container-person-details-textbox-skill-{skill.Id}", skillTextBox);
            }

            _resources.Set("container-person-details-current-person", person);

            container.IsVisible = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            foreach (var updateable in _updateables)
            {
                updateable.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            foreach (var renderable in _renderables.OrderBy(r => r.ZIndex))
            {
                if (!renderable.IsVisible) continue;

                renderable.Render(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler)
        {
            var metadata = _resources.Get<ButtonMetadata>("button-metadata-default");

            var button = new Button(metadata)
            {
                Text = text,
                Destination = new Rectangle(x, y, width, height)
            };

            button.Clicked += handler;

            _updateables.Add(button);
            _renderables.Add(button);

            return button;
        }
    }
}