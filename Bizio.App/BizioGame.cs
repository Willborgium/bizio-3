using Bizio.App.Game;
using Bizio.App.Services;
using Bizio.App.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Specialized;
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
                Position = new Vector2(1700, 50),
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Vertical
            };
            _visualRoot.AddChild(sampleContainer);

            _logger.IsVisible = false;

            sampleContainer.AddChild(CreateButton("Logger", 0, 0, 200, 50, ToggleDebugInfo));
            sampleContainer.AddChild(CreateButton("Snapshot", 0, 0, 200, 50, LogSnapshot));
            sampleContainer.AddChild(CreateButton("New Game", 0, 0, 200, 50, StartNewGame));
            sampleContainer.AddChild(CreateButton("My Company", 0, 0, 200, 50, ToggleMyCompany));
            sampleContainer.AddChild(CreateButton("People", 0, 0, 200, 50, TogglePeopleList));
            sampleContainer.AddChild(CreateButton("Projects", 0, 0, 200, 50, ToggleProjectsList));

            var headlineContainer = new StackContainer
            {
                Position = new Vector2(500, 0),
                IsVisible = false,
                Padding = new Vector4(50, 20, 50, 20),
                Direction = LayoutDirection.Horizontal
            };
            _visualRoot.AddChild(headlineContainer);
            _resources.Set("headline-container", headlineContainer);

            var currentTurn = new TextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font
            };
            headlineContainer.AddChild(currentTurn);
            _resources.Set("current-turn-textbox", currentTurn);

            var companyName = new TextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font,

            };
            headlineContainer.AddChild(companyName);
            _resources.Set("company-name-textbox", companyName);

            var companyMoney = new TextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font
            };
            headlineContainer.AddChild(companyMoney);
            _resources.Set("company-money-textbox", companyMoney);

            var employeeCount = new LabeledTextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font,
                Label = "Employees: "
            };
            headlineContainer.AddChild(employeeCount);
            _resources.Set("employee-count-textbox", employeeCount);

            var projectCount = new LabeledTextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font,
                Label = "Projects: "
            };
            headlineContainer.AddChild(projectCount);
            _resources.Set("project-count-textbox", projectCount);

            var nextTurnButton = CreateButton("Next Turn", 0, 0, 200, 50, NextTurn);
            _resources.Set("next-turn-button", nextTurnButton);
            headlineContainer.AddChild(nextTurnButton);

            _visualRoot.AddChild(CreatePeopleContainer());
            _visualRoot.AddChild(CreateProjectsContainer());
            _visualRoot.AddChild(CreateMyCompanyContainer());
            _visualRoot.AddChild(CreateMyCompanyProjectsContainer());

            CloseAllContainers();

            _dataService.Initialize();

            _logger.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            _logger.Info("Initialization complete");
        }

        // Utilities

        private void LogSnapshot(object sender, EventArgs e)
        {
            var renderableCount = _visualRoot.GetChildCount<IRenderable>(true);
            var updateableCount = _visualRoot.GetChildCount<IUpdateable>(true);

            _logger.Info("[BEGIN SNAPSHOT]");
            _logger.Info($"Renderables: {renderableCount}");
            _logger.Info($"Updateables: {updateableCount}");
            _logger.Info("[END SNAPSHOT]");
        }

        private void ToggleDebugInfo(object sender, EventArgs e)
        {
            _logger.IsVisible = !_logger.IsVisible;
            _logger.Info($"Logger toggled: {_logger.IsVisible}");
        }

        private static readonly string[] ContainerNames = new[]
        {
            "container-people",
            "container-person-details",
            "container-projects",
            "container-project-details",
            "container-my-company",
            "container-my-company-projects",
            "container-my-company-project-details",
        };

        private void CloseAllContainers(params string[] except)
        {
            foreach (var name in ContainerNames)
            {
                if (except?.Contains(name) == true)
                {
                    continue;
                }

                var renderable = _resources.Get<IRenderable>(name);

                if (renderable != null)
                {
                    renderable.IsVisible = false;
                }   
            }
        }

        private void ToggleContainer(string name, params string[] except)
        {
            CloseAllContainers(except.Append(name).ToArray());

            var container = _resources.Get<IRenderable>(name);
            container.IsVisible = !container.IsVisible;
        }

        // Game state management

        private void StartNewGame(object sender, EventArgs e)
        {
            _dataService.InitializeNewGame();

            _logger.Info($"Game ID: {_dataService.CurrentGame.GameId}");
            _logger.Info($"Person count: {_dataService.CurrentGame.People.Count}");

            var peopleContainer = _resources.Get<IContainer>("container-people");

            // Convert to method
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

            _logger.Info($"Project count: {_dataService.CurrentGame.Projects.Count}");

            var projectContainer = _resources.Get<IContainer>("container-projects");

            foreach (var project in _dataService.CurrentGame.Projects)
            {
                AddProject(project, projectContainer);
            }

            _dataService.CurrentGame.Projects.CollectionChanged += OnProjectsChanged;

            _dataService.CurrentGame.PlayerCompany.Projects.CollectionChanged += OnCompanyProjectsChanged;

            var headlineContainer = _resources.Get<IRenderable>("headline-container");
            headlineContainer.IsVisible = true;

            UpdateHeadlineContainer();
        }

        private void NextTurn(object sender, EventArgs e)
        {
            _dataService.CurrentGame.Turn++;

            var currentTurnTextBox = _resources.Get<TextBox>("current-turn-textbox");
            currentTurnTextBox.IsVisible = true;
            currentTurnTextBox.Text = $"Turn {_dataService.CurrentGame.Turn}";

            UpdateHeadlineContainer();
        }

        private void UpdateHeadlineContainer()
        {
            var currentTurnTextBox = _resources.Get<TextBox>("current-turn-textbox");
            currentTurnTextBox.Text = $"Turn {_dataService.CurrentGame.Turn}";

            var companyNameTextBox = _resources.Get<TextBox>("company-name-textbox");
            companyNameTextBox.Text = _dataService.CurrentGame.PlayerCompany.Name;

            var companyMoneyTextBox = _resources.Get<TextBox>("company-money-textbox");
            companyMoneyTextBox.Text = $"${_dataService.CurrentGame.PlayerCompany.Money:0.00}";

            var employeeCountTextBox = _resources.Get<LabeledTextBox>("employee-count-textbox");
            employeeCountTextBox.Text = $"{_dataService.CurrentGame.PlayerCompany.Employees.Count}";

            var projectCountTextBox = _resources.Get<LabeledTextBox>("project-count-textbox");
            projectCountTextBox.Text = $"{_dataService.CurrentGame.PlayerCompany.Projects.Count}";
        }

        // Projects

        private void OnProjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var projectContainer = _resources.Get<IContainer>("container-projects");

            if (e.OldItems != null)
            {
                foreach (Project project in e.OldItems)
                {
                    _logger.Info($"Removing project {project.Id}: {project.Name}");
                    RemoveProject(project, projectContainer);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Project project in e.NewItems)
                {
                    _logger.Info($"Adding project {project.Id}: {project.Name}");
                    AddProject(project, projectContainer);
                }
            }

            var details = _resources.Get<IContainer>("container-project-details");
            details.IsVisible = false;
        }

        private void AddProject(Project project, IContainer container)
        {
            var button = CreateButton(project.Name, ToggleProject, new DataEventArgs<Project>(project));
            button.Locator = $"project-accept-button-{project.Id}";
            container.AddChild(button);
        }

        private static void RemoveProject(Project project, IContainer container)
        {
            var child = container.FindChild($"project-accept-button-{project.Id}");
            container.RemoveChild(child);
        }

        // People

        private void TogglePeopleList(object sender, EventArgs e) => ToggleContainer("container-people");

        private void TogglePerson(Person person)
        {
            var previousContainer = _resources.Get<VisualContainer>("container-person-details");
            var previousPerson = _resources.Get<Person>("previous-person");

            _visualRoot.RemoveChild(previousContainer);

            if (person == null || person == previousPerson)
            {
                _resources.Set("previous-person", null);
                _resources.Set("container-person-details", null);
                return;
            }

            _resources.Set("previous-person", person);

            var currentContainer = CreatePersonDetailsContainer(person);

            _visualRoot.AddChild(currentContainer);
        }

        private IRenderable CreatePeopleContainer()
        {
            var container = new StackContainer
            {
                Position = new Vector2(0, 100),
                Padding = new Vector4(20, 5, 20, 5)
            };
            _resources.Set("container-people", container);

            return container;
        }

        private IRenderable CreatePersonDetailsContainer(Person person)
        {
            var root = new VisualContainer
            {
                IsVisible = true,
                Position = new Vector2(400, 100)
            };

            _resources.Set("container-person-details", root);

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
            hireButton.IsEnabled = !_dataService.CurrentGame.PlayerCompany.Employees.Any(e => e.PersonId == person.Id);
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

        private void HireCurrentPerson(object sender, EventArgs e)
        {
            var person = _resources.Get<Person>("previous-person");

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

            var hireButton = sender as Button;
            hireButton.IsEnabled = false;
        }

        // Projects

        private void ToggleProjectsList(object sender, EventArgs e) => ToggleContainer("container-projects");

        private void ToggleProject(object sender, DataEventArgs<Project> args)
        {
            var previousContainer = _resources.Get<VisualContainer>("container-project-details");

            var previousProject = _resources.Get<Project>("previous-project");

            _visualRoot.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousProject)
            {
                _resources.Set("previous-project", null);
                _resources.Set("container-project-details", null);
                return;
            }

            _resources.Set("previous-project", args.Data);

            var currentContainer = CreateProjectDetailsContainer(args.Data);

            _visualRoot.AddChild(currentContainer);
        }

        private IRenderable CreateProjectsContainer()
        {
            var container = new StackContainer
            {
                Position = new Vector2(0, 100),
                Padding = new Vector4(20, 5, 20, 5)
            };
            _resources.Set("container-projects", container);

            return container;
        }

        private IContainer CreateProjectDetailsContainer(Project project)
        {
            var root = new VisualContainer
            {
                IsVisible = true,
                Position = new Vector2(400, 100)
            };

            _resources.Set("container-project-details", root);

            var font = _resources.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 0),
                Text = project.Name,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            var descriptionLabel = new TextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 35),
                Text = project.Description,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(descriptionLabel);

            var valueLabel = new LabeledTextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 70),
                Label = "Pays",
                LabelWidth = 100,
                Text = $"${project.Value:0.00}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(valueLabel);

            var dueDateLabel = new LabeledTextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 105),
                Label = "Due on turn",
                LabelWidth = 100,
                Text = $"{project.TurnDue}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(dueDateLabel);

            var acceptButton = CreateButton("Accept", 0, 140, 300, 50, OnAcceptProject, new DataEventArgs<Project>(project));

            root.AddChild(acceptButton);

            var requirementsLabel = new TextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 200),
                Font = font,
                Color = Color.Black,
                Text = $"Requirements ({project.Requirements.Sum(r => r.TargetAmount):0.0})"
            };

            root.AddChild(requirementsLabel);

            var requirements = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(50, 235),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };

            root.AddChild(requirements);

            foreach (var requirement in project.Requirements)
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == requirement.SkillId);

                var requirementLabel = new LabeledTextBox
                {
                    IsVisible = true,
                    Font = font,
                    Color = Color.Black,
                    Label = skill.Name,
                    LabelWidth = 75,
                    Text = $"{requirement.TargetAmount:0.0}"
                };

                requirements.AddChild(requirementLabel);
            }

            return root;
        }

        private void OnAcceptProject(object sender, EventArgs e)
        {
            var project = (e as DataEventArgs<Project>).Data;

            _dataService.CurrentGame.Projects.Remove(project);
            _dataService.CurrentGame.PlayerCompany.Projects.Add(project);
        }

        // My Company

        private void ToggleMyCompany(object sender, EventArgs e) => ToggleContainer("container-my-company");

        private IRenderable CreateMyCompanyContainer()
        {
            var myCompanyContainer = new VisualContainer();
            _resources.Set("container-my-company", myCompanyContainer);

            var buttonsContainer = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(0, 100),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };

            myCompanyContainer.AddChild(buttonsContainer);

            var projectsButton = CreateButton("Projects", ToggleCompanyProjects);
            buttonsContainer.AddChild(projectsButton);

            return myCompanyContainer;
        }

        private IContainer CreateMyCompanyProjectsContainer()
        {
            var container = new StackContainer
            {
                Position = new Vector2(400, 100),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };
            _resources.Set("container-my-company-projects", container);
            return container;
        }

        private void ToggleCompanyProjects(object sender, EventArgs e) => ToggleContainer("container-my-company-projects", "container-my-company");

        private void ToggleCompanyProject(object sender, DataEventArgs<Project> args)
        {
            var previousContainer = _resources.Get<VisualContainer>("container-my-company-project-details");

            var previousProject = _resources.Get<Project>("previous-my-company-project");

            _visualRoot.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousProject)
            {
                _resources.Set("previous-my-company-project", null);
                _resources.Set("container-my-company-project-details", null);
                return;
            }

            _resources.Set("previous-my-company-project", args.Data);

            var currentContainer = CreateCompanyProjectDetailsContainer(args.Data);

            _visualRoot.AddChild(currentContainer);
        }

        private void OnCompanyProjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var container = _resources.Get<IContainer>("container-my-company-projects");

            if (e.OldItems != null)
            {
                foreach (Project project in e.OldItems)
                {
                    _logger.Info($"Removing player project {project.Id}: {project.Name}");
                    var child = container.FindChild($"my-company-view-project-{project.Id}");
                    container.RemoveChild(child);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Project project in e.NewItems)
                {
                    _logger.Info($"Adding player project {project.Id}: {project.Name}");
                    var button = CreateButton(project.Name, ToggleCompanyProject, new DataEventArgs<Project>(project));
                    button.Locator = $"my-company-view-project-{project.Id}";
                    container.AddChild(button);
                }
            }
        }

        private IContainer CreateCompanyProjectDetailsContainer(Project project)
        {
            var root = new VisualContainer
            {
                IsVisible = true,
                Position = new Vector2(800, 100)
            };

            _resources.Set("container-my-company-project-details", root);

            var font = _resources.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 0),
                Text = project.Name,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            var descriptionLabel = new TextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 35),
                Text = project.Description,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(descriptionLabel);

            var valueLabel = new LabeledTextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 70),
                Label = "Pays",
                LabelWidth = 100,
                Text = $"${project.Value:0.00}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(valueLabel);

            var dueDateLabel = new LabeledTextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 105),
                Label = "Due on turn",
                LabelWidth = 100,
                Text = $"{project.TurnDue}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(dueDateLabel);

            var requirementsLabel = new TextBox
            {
                IsVisible = true,
                Position = new Vector2(0, 200),
                Font = font,
                Color = Color.Black,
                Text = $"Requirements ({project.Requirements.Sum(r => r.TargetAmount):0.0})"
            };

            root.AddChild(requirementsLabel);

            var requirements = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(50, 235),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };

            root.AddChild(requirements);

            foreach (var requirement in project.Requirements)
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == requirement.SkillId);

                var requirementLabel = new LabeledTextBox
                {
                    IsVisible = true,
                    Font = font,
                    Color = Color.Black,
                    Label = skill.Name,
                    LabelWidth = 75,
                    Text = $"{requirement.TargetAmount:0.0}"
                };

                requirements.AddChild(requirementLabel);
            }

            return root;
        }

        // Core

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _visualRoot.Update();

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

        private Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null)
        {
            var metadata = _resources.Get<ButtonMetadata>("button-metadata-default");

            var button = new Button(metadata)
            {
                Text = text,
                Position = new Vector2(x, y),
                Dimensions = new Vector2(width, height),
                Args = args ?? EventArgs.Empty
            };

            button.Clicked += handler;

            return button;
        }

        private Button CreateButton<T>(string text, EventHandler<T> handler, T args = null)
            where T : EventArgs
        {
            return CreateButton(text, 0, 0, 300, 50, (s, e) => handler?.Invoke(s, e as T), args);
        }

        private Button CreateButton(string text, EventHandler handler, EventArgs args = null)
        {
            return CreateButton(text, 0, 0, 300, 50, handler, args);
        }
    }
}