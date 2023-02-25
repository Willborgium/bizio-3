using Bizio.App.Game;
using Bizio.App.Services;
using Bizio.App.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;

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
                PreferredBackBufferHeight = 1080,
                IsFullScreen = true
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
            DebuggingService.Font = font;

            var pixel = Content.Load<Texture2D>("pixel");
            _resources.Set("texture-pixel", pixel);
            DebuggingService.PixelTexture = pixel;

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

            _visualRoot.AddChild(CreateMenuContainer());
            _visualRoot.AddChild(CreateDebugContainer());
            _visualRoot.AddChild(CreateHeadlineContainer());
            _visualRoot.AddChild(CreatePeopleContainer());
            _visualRoot.AddChild(CreateProjectsContainer());
            _visualRoot.AddChild(CreateMyCompanyContainer());
            _visualRoot.AddChild(CreateMyCompanyProjectsContainer());
            _visualRoot.AddChild(CreateMyCompanyEmployeesContainer());

            CloseAllContainers();

            _dataService.Initialize();

            _logger.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            _logger.Info("Initialization complete");
        }

        private IContainer CreateDebugContainer()
        {
            var container = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Horizontal
            };

            container.AddChild(CreateButton("Logger", 0, 0, 200, 50, ToggleDebugInfo));
            container.AddChild(CreateButton("Snapshot", 0, 0, 200, 50, LogSnapshot));
            container.AddChild(CreateButton("Outlines", 0, 0, 200, 50, ToggleDebuggingOutlines));

            var x = _graphics.PreferredBackBufferWidth - container.Dimensions.X;
            var y = _graphics.PreferredBackBufferHeight - container.Dimensions.Y;

            container.Position = new Vector2(x, y);

            return container;
        }

        private void ToggleDebuggingOutlines(object sender, EventArgs e)
        {
            var isEnabled = DebuggingService.IsEnabled(DebugFlag.RenderableOutlines);
            DebuggingService.Set(DebugFlag.RenderableOutlines, !isEnabled);
        }

        private IContainer CreateMenuContainer()
        {
            var container = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Vertical
            };

            container.AddChild(CreateButton("New Game", 0, 0, 200, 50, StartNewGame));
            container.AddChild(CreateButton("My Company", 0, 0, 200, 50, ToggleMyCompany));
            container.AddChild(CreateButton("People", 0, 0, 200, 50, TogglePeopleList));
            container.AddChild(CreateButton("Projects", 0, 0, 200, 50, ToggleProjectsList));

            var x = _graphics.PreferredBackBufferWidth - container.Dimensions.X;
            var y = (_graphics.PreferredBackBufferHeight - container.Dimensions.Y) / 2;

            container.Position = new Vector2(x, y);

            return container;
        }

        private IContainer CreateHeadlineContainer()
        {
            var font = _resources.Get<SpriteFont>("font-default");

            var container = new StackContainer
            {
                Position = new Vector2(500, 0),
                IsVisible = false,
                Padding = new Vector4(50, 20, 50, 20),
                Direction = LayoutDirection.Horizontal
            };
            _resources.Set("headline-container", container);

            var currentTurn = new TextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font
            };
            container.AddChild(currentTurn);
            _resources.Set("current-turn-textbox", currentTurn);

            var companyName = new TextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font,

            };
            container.AddChild(companyName);
            _resources.Set("company-name-textbox", companyName);

            var companyMoney = new TextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font
            };
            container.AddChild(companyMoney);
            _resources.Set("company-money-textbox", companyMoney);

            var employeeCount = new LabeledTextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font,
                Label = "Employees: "
            };
            container.AddChild(employeeCount);
            _resources.Set("employee-count-textbox", employeeCount);

            var projectCount = new LabeledTextBox
            {
                IsVisible = true,
                Color = Color.Black,
                Font = font,
                Label = "Projects: "
            };
            container.AddChild(projectCount);
            _resources.Set("project-count-textbox", projectCount);

            var nextTurnButton = CreateButton("Next Turn", 0, 0, 200, 50, NextTurn);
            _resources.Set("next-turn-button", nextTurnButton);
            container.AddChild(nextTurnButton);

            return container;
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
            "container-my-company-employees",
            "container-my-company-employee-details",
            "container-my-company-projects",
            "container-my-company-project-details",
            "container-my-company-project-allocations"
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
            _dataService.InitializeNewGame(
                g =>
                {
                    g.Projects.CollectionChanged += OnProjectsChanged;
                    g.People.CollectionChanged += OnPeopleChanged;
                },
                c =>
                {
                    c.Projects.CollectionChanged += OnCompanyProjectsChanged;
                    c.Employees.CollectionChanged += OnCompanyEmployeesChanged;
                    c.Allocations.CollectionChanged += OnCompanyAllocationsChanged;
                }
            );

            _logger.Info($"Game ID: {_dataService.CurrentGame.GameId}");
            _logger.Info($"Person count: {_dataService.CurrentGame.People.Count}");
            _logger.Info($"Company count: {_dataService.CurrentGame.Companies.Count}");
            _logger.Info($"Project count: {_dataService.CurrentGame.Projects.Count}");

            // Maybe make Companies observable?
            foreach (var company in _dataService.CurrentGame.Companies)
            {
                var founder = company.Founder.Person;
                _logger.Info($"Company {company.Id}: {company.Name} founded by {founder.FirstName} {founder.LastName}");
            }

            var headlineContainer = _resources.Get<IRenderable>("headline-container");
            headlineContainer.IsVisible = true;

            (sender as Button).IsEnabled = false;

            UpdateHeadlineContainer();
        }

        private void NextTurn(object sender, EventArgs e)
        {
            foreach (var company in _dataService.CurrentGame.Companies)
            {
                // Charge employee salary
                foreach (var employee in company.Employees)
                {
                    company.Money -= employee.Salary;
                }

                // Apply skills to projects
                foreach (var allocation in company.Allocations)
                {
                    foreach (var requirement in allocation.Project.Requirements)
                    {
                        if (requirement.CurrentAmount == requirement.TargetAmount)
                        {
                            continue;
                        }

                        var skill = allocation.Employee.Person.Skills.FirstOrDefault(s => s.SkillId== requirement.SkillId); ;

                        if (skill == default)
                        {
                            continue;
                        }

                        requirement.CurrentAmount += skill.Value;

                        requirement.CurrentAmount = Math.Min(requirement.CurrentAmount, requirement.TargetAmount);

                        // Increase skill due to usage
                        var rate = 0f;
                        switch (skill.LearnRate)
                        {
                            case PersonSkillLearnRate.VerySlow:
                                rate = 1f;
                                break;
                            case PersonSkillLearnRate.Slow:
                                rate = 2f;
                                break;
                            case PersonSkillLearnRate.Average:
                                rate = 3f;
                                break;
                            case PersonSkillLearnRate.Fast:
                                rate = 4f;
                                break;
                            case PersonSkillLearnRate.VeryFast:
                                rate = 5f;
                                break;
                        }
                        skill.Value += rate;
                    }
                }

                foreach (var project in company.Projects)
                {
                    if (project.Status != ProjectStatus.InProgress)
                    {
                        continue;
                    }

                    var isComplete = project.Requirements.All(r => r.CurrentAmount >= r.TargetAmount);

                    if (isComplete)
                    {
                        company.Money += project.Value;
                        project.Status = ProjectStatus.Completed;
                        continue;
                    }

                    if (project.TurnDue == _dataService.CurrentGame.Turn)
                    {
                        project.Status = ProjectStatus.Failed;

                        var toRemove = company.Allocations.Where(a => a.Project == project).ToList();

                        foreach (var allocation in toRemove)
                        {
                            company.Allocations.Remove(allocation);
                        }

                        continue;
                    }
                }
            }
            
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

        private void OnPeopleChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var container = _resources.Get<IContainer>("container-people");

            if (e.OldItems != null)
            {
                foreach (Person person in e.OldItems)
                {
                    _logger.Info($"Removing person {person.Id}: {person.FullName}");
                    RemovePerson(person, container);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Person person in e.NewItems)
                {
                    _logger.Info($"Adding person {person.Id}: {person.FullName}");
                    AddPerson(person, container);
                }
            }
        }

        private void AddPerson(Person person, IContainer container)
        {
            _logger.Info($"Adding person {person.Id}: {person.FullName}");
            var button = CreateButton($"{person.FullName}", TogglePerson, new DataEventArgs<Person>(person));
            button.Locator = $"person-{person.Id}";
            container.AddChild(button);
        }

        private static void RemovePerson(Person person, IContainer container)
        {
            var child = container.FindChild($"person-{person.Id}");
            container.RemoveChild(child);
        }

        private void TogglePeopleList(object sender, EventArgs e) => ToggleContainer("container-people");

        private void TogglePerson(object sender, DataEventArgs<Person> args)
        {
            var previousContainer = _resources.Get<IContainer>("container-person-details");
            var previousPerson = _resources.Get<Person>("previous-person");

            _visualRoot.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousPerson)
            {
                _resources.Set("previous-person", null);
                _resources.Set("container-person-details", null);
                return;
            }

            _resources.Set("previous-person", args.Data);

            var currentContainer = CreatePersonDetailsContainer(args.Data);

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
            var root = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(400, 100),
                Padding = new Vector4(0, 10, 0, 10)
            };

            _resources.Set("container-person-details", root);

            var font = _resources.Get<SpriteFont>("font-default");

            // Name
            var nameLabel = new LabeledTextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
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
                Label = "Gender",
                LabelWidth = 75,
                Text = $"{person.Gender}"
            };
            root.AddChild(genderLabel);

            // Hire Button
            var hireButton = CreateButton("Hire", (s, e) => HirePerson(person));
            hireButton.Locator = $"person-hire-button-{person.Id}";
            root.AddChild(hireButton);

            // Skills
            var skillsLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Text = "Skills"
            };
            root.AddChild(skillsLabel);

            var skillsContainer = new StackContainer
            {
                IsVisible = true,
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
                    Position = new Vector2(75, 0),
                    Label = skill.Name,
                    LabelWidth = 50,
                    Text = $"{personSkill.Value:0.0}"
                };

                skillsContainer.AddChild(skillLabel);
            }

            return root;
        }

        private void HirePerson(Person person)
        {
            var employee = new Employee
            {
                Person = person,
                Salary = new Random().Next(1, 100)
            };

            _dataService.CurrentGame.PlayerCompany.Employees.Add(employee);
            _dataService.CurrentGame.People.Remove(person);

            var details = _resources.Get<IContainer>("container-person-details");
            details.IsVisible = false;
        }

        // Projects

        private void ToggleProjectsList(object sender, EventArgs e) => ToggleContainer("container-projects");

        private void ToggleProject(object sender, DataEventArgs<Project> args)
        {
            var previousContainer = _resources.Get<IContainer>("container-project-details");

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
            var root = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(400, 100),
                Padding = new Vector4(0, 10, 0, 10)
            };

            _resources.Set("container-project-details", root);

            var font = _resources.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                IsVisible = true,
                Text = project.Name,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            var descriptionLabel = new TextBox
            {
                IsVisible = true,
                Text = project.Description,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(descriptionLabel);

            var valueLabel = new LabeledTextBox
            {
                IsVisible = true,
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
                Label = "Due on turn",
                LabelWidth = 100,
                Text = $"{project.TurnDue}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(dueDateLabel);

            var acceptButton = CreateButton("Accept", OnAcceptProject, new DataEventArgs<Project>(project));

            root.AddChild(acceptButton);

            var requirementsLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Text = $"Requirements ({project.Requirements.Sum(r => r.TargetAmount):0.0})"
            };

            root.AddChild(requirementsLabel);

            var requirements = new StackContainer
            {
                IsVisible = true,
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
                    Position = new Vector2(50, 0),
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

            var details = _resources.Get<IContainer>("container-project-details");
            details.IsVisible = false;
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

            var employeesButton = CreateButton("Employees", ToggleCompanyEmployees);
            buttonsContainer.AddChild(employeesButton);

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

        private IContainer CreateMyCompanyEmployeesContainer()
        {
            var container = new StackContainer
            {
                Position = new Vector2(400, 100),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };
            _resources.Set("container-my-company-employees", container);
            return container;
        }

        private void ToggleCompanyProjects(object sender, EventArgs e) => ToggleContainer("container-my-company-projects", "container-my-company");

        private void ToggleCompanyEmployees(object sender, EventArgs e) => ToggleContainer("container-my-company-employees", "container-my-company");

        private void ToggleCompanyProject(object sender, DataEventArgs<Project> args)
        {
            var previousContainer = _resources.Get<IContainer>("container-my-company-project-details");

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

        private void ToggleCompanyEmployee(object sender, DataEventArgs<Employee> args)
        {
            var previousContainer = _resources.Get<IContainer>("container-my-company-employee-details");

            var previousEmployee = _resources.Get<Employee>("previous-my-company-employee");

            _visualRoot.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousEmployee)
            {
                _resources.Set("previous-my-company-employee", null);
                _resources.Set("container-my-company-employee-details", null);
                return;
            }

            _resources.Set("previous-my-company-employee", args.Data);

            var currentContainer = CreateCompanyEmployeeDetailsContainer(args.Data);

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

        private void OnCompanyEmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var container = _resources.Get<IContainer>("container-my-company-employees");

            if (e.OldItems != null)
            {
                foreach (Employee employee in e.OldItems)
                {
                    _logger.Info($"Removing player employee {employee.Person.Id}");
                    var child = container.FindChild($"my-company-view-employee-{employee.Person.Id}");
                    container.RemoveChild(child);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Employee employee in e.NewItems)
                {
                    _logger.Info($"Adding player employee {employee.Person.Id}");
                    var button = CreateButton(employee.Person.FullName, ToggleCompanyEmployee, new DataEventArgs<Employee>(employee));
                    button.Locator = $"my-company-view-employee-{employee.Person.Id}";
                    container.AddChild(button);
                }
            }
        }

        private IContainer CreateCompanyProjectDetailsContainer(Project project)
        {
            var projectContainer = new VisualContainer
            {
                IsVisible = true,
                Position = new Vector2(800, 100)
            };

            _resources.Set("container-my-company-project-details", projectContainer);

            var root = new StackContainer
            {
                IsVisible = true,
                Padding = new Vector4(0, 10, 0, 10)
            };

            projectContainer.AddChild(root);

            var font = _resources.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                IsVisible = true,
                Text = project.Name,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            var descriptionLabel = new TextBox
            {
                IsVisible = true,
                Text = project.Description,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(descriptionLabel);

            var valueLabel = new LabeledTextBox
            {
                IsVisible = true,
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
                Label = "Due on turn",
                LabelWidth = 100,
                Text = $"{project.TurnDue}",
                Color = Color.Black,
                Font = font
            };

            if (project.Status != ProjectStatus.InProgress)
            {
                var statusLabel = new LabeledTextBox
                {
                    IsVisible = true,
                    Label = "Status",
                    LabelWidth = 100,
                    Text = $"{project.Status}",
                    Color = Color.Black,
                    Font = font
                };

                root.AddChild(statusLabel);
            }

            if (project.Status == ProjectStatus.InProgress)
            {
                var allocationsButton = CreateButton("Allocations", ToggleCompanyProjectAllocationsContainer, new DataEventArgs<Project>(project));
                root.AddChild(allocationsButton);
            }

            var requirementsLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Text = $"Requirements ({project.Requirements.Sum(r => r.TargetAmount):0.0})"
            };

            root.AddChild(requirementsLabel);

            var requirements = new StackContainer
            {
                IsVisible = true,
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
                    Position = new Vector2(50, 0),
                    Label = skill.Name,
                    LabelWidth = 75,
                    Text = $"{requirement.CurrentAmount:0.0} / {requirement.TargetAmount:0.0}"
                };

                requirements.AddChild(requirementLabel);
            }

            return projectContainer;
        }

        private void ToggleCompanyProjectAllocationsContainer(object sender, DataEventArgs<Project> e)
        {
            var projectContainer = _resources.Get<IContainer>("container-my-company-project-details");

            var previousContainer = projectContainer.FindChild("container-my-company-project-allocations");
            projectContainer.RemoveChild(previousContainer);

            var previousData = _resources.Get<Project>("my-company-project-allocations-current");
            if (previousData == e.Data)
            {
                _resources.Set("my-company-project-allocations-current", null);
                return;
            }

            _resources.Set("my-company-project-allocations-current", e.Data);

            var container = CreateCompanyProjectAllocationsContainer(e.Data);

            projectContainer.AddChild(container);
        }

        private void OnCompanyAllocationsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var currentProject = _resources.Get<Project>("my-company-project-allocations-current");

            var currentProjectAllocationsContainer = _visualRoot.FindChild<ContainerBase>("container-my-company-project-allocations-current");

            if (args.OldItems != null)
            {
                var currentProjectEmployeesContainer = _visualRoot.FindChild<ContainerBase>("container-my-company-project-employees");

                foreach (Allocation allocation in args.OldItems)
                {
                    if (allocation.Project != currentProject)
                    {
                        continue;
                    }

                    _logger.Info($"Adding company project allocation {allocation.Employee.Person.Id} -> {allocation.Project.Id}");
                    var child = currentProjectAllocationsContainer.FindChild($"project-allocation-{allocation.Employee.Person.Id}");
                    currentProjectAllocationsContainer.RemoveChild(child);

                    currentProjectEmployeesContainer.AddChild(CreateButton(allocation.Employee.Person.FullName, (s, e) =>
                    {
                        _dataService.CurrentGame.PlayerCompany.Allocations.Add(new Allocation
                        {
                            Employee = allocation.Employee,
                            Project = allocation.Project,
                            Percent = 1f
                        });
                        currentProjectEmployeesContainer.RemoveChild(s);
                    }));
                    
                }
            }

            if (args.NewItems != null)
            {
                foreach (Allocation allocation in args.NewItems)
                {
                    if (allocation.Project != currentProject)
                    {
                        continue;
                    }

                    _logger.Info($"Adding company project allocation {allocation.Employee.Person.Id} -> {allocation.Project.Id}");
                    var uiAllocation = CreateCompanyProjectAllocation(allocation);
                    currentProjectAllocationsContainer.AddChild(uiAllocation);
                }
            }
        }

        private IRenderable CreateCompanyProjectAllocation(Allocation allocation)
        {
            var container = new StackContainer
            {
                IsVisible = true,
                Padding = new Vector4(10, 0, 10, 0),
                Direction = LayoutDirection.Horizontal,
                Locator = $"project-allocation-{allocation.Employee.Person.Id}",
            };

            var font = _resources.Get<SpriteFont>("font-default");

            container.AddChild(new TextBox
            {
                IsVisible = true,
                Text = allocation.Employee.Person.FullName,
                Font = font,
                Color = Color.Black
            });

            var value = new TextBox
            {
                IsVisible = true,
                Text = $"{allocation.Percent:P0}",
                Font = font,
                Color = Color.Black
            };

            var minus = CreateButton("-", 0, 0, 35, 35, (s, e) =>
            {
                allocation.Percent -= .1f;
                if (allocation.Percent < .1f)
                {
                    allocation.Percent = .1f;
                }
                value.Text = $"{allocation.Percent:P0}";
            });
            container.AddChild(minus);

            container.AddChild(value);

            var plus = CreateButton("+", 0, 0, 35, 35, (s, e) =>
            {
                allocation.Percent += .1f;
                if (allocation.Percent > 1f)
                {
                    allocation.Percent = 1f;
                }
                value.Text = $"{allocation.Percent:P0}";
            });
            container.AddChild(plus);

            var delete = CreateButton("X", 0, 0, 35, 35, (s, e) =>
            {
                _dataService.CurrentGame.PlayerCompany.Allocations.Remove(allocation);
            });
            container.AddChild(delete);

            return container;
        }

        private IContainer CreateCompanyProjectAllocationsContainer(Project project)
        {
            var container = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(300, 0),
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-my-company-project-allocations"
            };

            var currentContainer = new StackContainer
            {
                IsVisible = true,
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-my-company-project-allocations-current"
            };

            container.AddChild(currentContainer);

            var allocations = _dataService.CurrentGame.PlayerCompany.Allocations.Where(a => a.Project == project);

            foreach (var allocation in allocations)
            {
                currentContainer.AddChild(CreateCompanyProjectAllocation(allocation));
            }

            var employeesContainer = new StackContainer
            {
                IsVisible = true,
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-my-company-project-employees"
            };

            container.AddChild(employeesContainer);

            var employees = _dataService.CurrentGame.PlayerCompany.Employees.Where(e => !allocations.Any(a => a.Employee == e));

            foreach (var employee in employees)
            {
                employeesContainer.AddChild(CreateButton(employee.Person.FullName, (s, e) =>
                {
                    _dataService.CurrentGame.PlayerCompany.Allocations.Add(new Allocation
                    {
                        Employee = employee,
                        Project = project,
                        Percent = 1f
                    });
                    employeesContainer.RemoveChild(s);
                }));
            }

            return container;
        }

        private IContainer CreateCompanyEmployeeDetailsContainer(Employee employee)
        {
            var root = new StackContainer
            {
                IsVisible = true,
                Position = new Vector2(800, 100),
                Padding = new Vector4(0, 10, 0, 10)
            };

            _resources.Set("container-my-company-employee-details", root);

            var font = _resources.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                IsVisible = true,
                Text = employee.Person.FullName,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            // Gender
            var genderLabel = new LabeledTextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Label = "Gender",
                LabelWidth = 75,
                Text = $"{employee.Person.Gender}"
            };

            root.AddChild(genderLabel);

            // Salary
            var salaryLabel = new LabeledTextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Label = "Salary",
                LabelWidth = 75,
                Text = $"{employee.Salary}"
            };

            root.AddChild(salaryLabel);

            // Fire
            var fireButton = CreateButton("Fire", OnFireEmployee, new DataEventArgs<Employee>(employee));
            fireButton.IsEnabled = !employee.IsFounder;
            root.AddChild(fireButton);

            // Skills
            var skillsLabel = new TextBox
            {
                IsVisible = true,
                Font = font,
                Color = Color.Black,
                Text = "Skills"
            };

            root.AddChild(skillsLabel);

            var skillsContainer = new StackContainer
            {
                IsVisible = true,
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };
            root.AddChild(skillsContainer);

            foreach (var personSkill in employee.Person.Skills.OrderByDescending(s => s.Value))
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == personSkill.SkillId);

                var skillLabel = new LabeledTextBox
                {
                    IsVisible = true,
                    Font = font,
                    Color = Color.Black,
                    Position = new Vector2(75, 0),
                    Label = skill.Name,
                    LabelWidth = 50,
                    Text = $"{personSkill.Value:0.0}"
                };

                skillsContainer.AddChild(skillLabel);
            }

            return root;
        }

        private void OnFireEmployee(object sender, DataEventArgs<Employee> e)
        {
            _dataService.CurrentGame.PlayerCompany.Employees.Remove(e.Data);
            _dataService.CurrentGame.People.Add(e.Data.Person);

            ToggleCompanyEmployee(sender, e);
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