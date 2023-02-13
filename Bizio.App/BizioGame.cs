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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;

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

        private IContainer _visualRoot;

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
            _visualRoot = new VisualContainer
            {
                Position = new Vector2(0, 0)
            };

            _visualRoot.AddChild(_logger);

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

            var sampleContainer = new StackContainer
            {
                Position = new Vector2(500, 0),
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Horizontal
            };
            _visualRoot.AddChild(sampleContainer);

            _logger.IsVisible = false;

            sampleContainer.AddChild(CreateButton("Logger", 0, 0, 200, 50, ToggleDebugInfo));
            sampleContainer.AddChild(CreateButton("Snapshot", 0, 0, 200, 50, LogSnapshot));
            sampleContainer.AddChild(CreateButton("New Game", 0, 0, 200, 50, StartNewGame));
            sampleContainer.AddChild(CreateButton("People", 0, 0, 200, 50, TogglePeopleList));
            sampleContainer.AddChild(CreateButton("My Company", 0, 0, 200, 50, ToggleMyCompany));

            _visualRoot.AddChild(CreatePeopleContainer());
            _visualRoot.AddChild(CreateMyCompanyContainer());

            _dataService.Initialize();

            _logger.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            _logger.Info("Initialization complete");
        }

        private void LogSnapshot(object sender, EventArgs e)
        {
            var renderableCount = _visualRoot.GetChildCount(true);
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

        private IRenderable CreatePersonDetailsContainer(Person person)
        {
            var root = new VisualContainer
            {
                IsVisible = true,
                Position = new Vector2(400, 0)
            };

            _resources.Set("container-person-details", root);
            _renderables.Add(root);

            var font = _resources.Get<SpriteFont>("font-default");

            // Name
            var nameLabel = new LabeledTextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(0, 0),
                Label = "Name",
                LabelWidth = 75,
                Text = person.FullName
            };

            root.AddChild(nameLabel);

            // Gender
            var genderLabel = new LabeledTextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(0, 50),
                Label = "Gender",
                LabelWidth = 75,
                Text = $"{person.Gender}"
            };

            root.AddChild(genderLabel);

            // Hire Button
            var hireButton = CreateButton("Hire", 0, 100, 300, 50, HireCurrentPerson);
            hireButton.IsEnabled = _dataService.CurrentGame.PlayerCompany.Employees.Any(e => e.PersonId == person.Id);
            _resources.Set("container-person-details-button-hire", hireButton);
            root.AddChild(hireButton);

            // Skills
            var skillsLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Position = new Vector2(0, 175),
                Text = "Skills"
            };

            root.AddChild(skillsLabel);

            var skillsContainer = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(75, 200),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };
            root.AddChild(skillsContainer);

            foreach (var personSkill in person.Skills.OrderByDescending(s => s.Value))
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == personSkill.SkillId);

                var skillLabel = new LabeledTextBox
                {
                    IsVisible = true,
                    Font = font,
                    Color = Color.Black,
                    Position = new Vector2(0, 0),
                    Label = skill.Name,
                    LabelWidth = 50,
                    Text = $"{personSkill.Value:0.0}"
                };

                skillsContainer.AddChild(skillLabel);
            }

            return root;
        }

        private IRenderable CreatePeopleContainer()
        {
            var peopleContainer = new StackContainer
            {
                Position = new Vector2(0, 100),
                Padding = new Vector4(20, 5, 20, 5)
            };
            _resources.Set("container-people", peopleContainer);

            return peopleContainer;
        }

        private IRenderable CreateMyCompanyContainer()
        {
            var myCompanyContainer = new VisualContainer();
            _resources.Set("container-my-company", myCompanyContainer);
            return myCompanyContainer;
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
            var container = _resources.Get<StackContainer>("container-people");
            container.IsVisible = !container.IsVisible;
            _logger.Info($"People container toggled: {container.IsVisible}");
        }

        private void StartNewGame(object sender, EventArgs e)
        {
            _dataService.InitializeNewGame();

            _logger.Info($"Game ID: {_dataService.CurrentGame.GameId}");
            _logger.Info($"Person count: {_dataService.CurrentGame.People.Count}");

            var peopleContainer = _resources.Get<StackContainer>("container-people");

            foreach (var person in _dataService.CurrentGame.People)
            {
                _logger.Info($"Person {person.Id}: {person.FirstName} {person.LastName}, Skills: {person.Skills.Count}");

                var button = CreateButton($"{person.FirstName} {person.LastName}", 0, 0, 300, 50, (s, e) => TogglePerson(person));
                peopleContainer.AddChild(button);
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
            var previousContainer = _resources.Get<VisualContainer>("container-person-details");

            var previousPerson = _resources.Get<Person>("previous-person");

            // TODO: when removing children from the visual tree
            // check if they are updateable OR have updateable children
            // and remove those from the logical tree as well
            // buttons are the current problem. Once created, they exist
            // 4ever
            _visualRoot.RemoveChild(previousContainer);

            if (person == null || person == previousPerson)
            {
                _resources.Set("previous-person", null);
                return;
            }

            _resources.Set("previous-person", person);

            var currentContainer = CreatePersonDetailsContainer(person);

            _visualRoot.AddChild(currentContainer);
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

            _visualRoot.Render(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler)
        {
            var metadata = _resources.Get<ButtonMetadata>("button-metadata-default");

            var button = new Button(metadata)
            {
                Text = text,
                Position = new Vector2(x, y),
                Dimensions = new Vector2(width, height)
            };

            button.Clicked += handler;

            _updateables.Add(button);

            return button;
        }
    }
}