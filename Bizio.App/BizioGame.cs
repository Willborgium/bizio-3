using Bizio.App.Game;
using Bizio.App.Services;
using Bizio.App.UI;
using Bizio.App.UI.Containers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Bizio.App
{
    public class BizioGame : Microsoft.Xna.Framework.Game, IRunnable
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly IResourceService _resourceService;
        private readonly IDataService _dataService;
        private readonly ILoggingService _loggingService;
        private readonly IUiService _uiService;

        public BizioGame(
            IResourceService resourceService,
            IDataService dataService,
            ILoggingService loggingService,
            IUiService uiService
            )
        {
            _resourceService = resourceService;
            _dataService = dataService;
            _loggingService = loggingService;
            _uiService = uiService;

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                IsFullScreen = false
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _uiService.AddChild(_loggingService);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var font = Content.Load<SpriteFont>("font-default");
            _resourceService.Set("font-default", font);
            DebuggingService.Font = font;

            var pixel = Content.Load<Texture2D>("pixel");
            _resourceService.Set("texture-pixel", pixel);
            DebuggingService.PixelTexture = pixel;

            _loggingService.Initialize(font, pixel);

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

            _resourceService.Set("button-spritesheet-default", buttonSpritesheet);
            _resourceService.Set("button-metadata-default", buttonMetadata);

            _uiService.AddChild(CreateMenuContainer());
            var debugContainer = new DebugContainer(_loggingService, _uiService);
            debugContainer.Initialize(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _uiService.AddChild(debugContainer);

            var gameContainer = new VisualContainer
            {
                IsVisible = false,
                Locator = "container-game"
            };
            _uiService.AddChild(gameContainer);

            gameContainer.Bindings.Add(Binding.Create(() => _dataService?.CurrentGame != null, x => gameContainer.IsVisible = x));

            gameContainer.AddChild(CreateHeadlineContainer());
            gameContainer.AddChild(CreatePeopleContainer());
            gameContainer.AddChild(CreateProjectsContainer());
            gameContainer.AddChild(CreateMyCompanyContainer());
            gameContainer.AddChild(CreateMyCompanyProjectsContainer());
            gameContainer.AddChild(CreateMyCompanyEmployeesContainer());

            CloseAllContainers();

            _dataService.Initialize();

            _loggingService.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            _loggingService.Info("Initialization complete");
        }

        private IContainer CreateMenuContainer()
        {
            var container = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Vertical,
                Locator = "container-menu"
            };

            container.AddChild(_uiService.CreateButton("New Game", 0, 0, 200, 50, StartNewGame));
            container.AddChild(_uiService.CreateButton("My Company", 0, 0, 200, 50, ToggleMyCompany));
            container.AddChild(_uiService.CreateButton("People", 0, 0, 200, 50, TogglePeopleList));
            container.AddChild(_uiService.CreateButton("Projects", 0, 0, 200, 50, ToggleProjectsList));

            var x = _graphics.PreferredBackBufferWidth - container.Dimensions.X;
            var y = (_graphics.PreferredBackBufferHeight - container.Dimensions.Y) / 2;

            container.Position = new Vector2(x, y);

            return container;
        }

        private IContainer CreateHeadlineContainer()
        {
            var font = _resourceService.Get<SpriteFont>("font-default");

            var container = new StackContainer
            {
                Position = new Vector2(500, 0),
                Padding = new Vector4(50, 20, 50, 20),
                Direction = LayoutDirection.Horizontal,
                Locator = "container-headline"
            };

            var currentTurn = new TextBox
            {
                Color = Color.Black,
                Font = font
            };
            currentTurn.Bind(x => x.Text = $"Turn {_dataService.CurrentGame?.Turn}");
            container.AddChild(currentTurn);

            var companyName = new TextBox
            {
                Color = Color.Black,
                Font = font
            };
            companyName.Bind(x => x.Text = _dataService.CurrentGame?.PlayerCompany?.Name);
            container.AddChild(companyName);

            var companyMoney = new TextBox
            {
                Color = Color.Black,
                Font = font
            };
            companyMoney.Bind(x => x.Text = $"${_dataService.CurrentGame?.PlayerCompany?.Money:0.00}");
            container.AddChild(companyMoney);

            var employeeCount = new LabeledTextBox
            {
                Color = Color.Black,
                Font = font,
                Label = "Employees: "
            };
            employeeCount.Bind(x => x.Text = $"{_dataService.CurrentGame?.PlayerCompany?.Employees.Count}");
            container.AddChild(employeeCount);

            var projectCount = new LabeledTextBox
            {
                Color = Color.Black,
                Font = font,
                Label = "Projects: "
            };
            projectCount.Bind(x => x.Text = $"{_dataService.CurrentGame?.PlayerCompany?.Projects.Count}");
            container.AddChild(projectCount);

            var nextTurnButton = _uiService.CreateButton("Next Turn", 0, 0, 200, 50, NextTurn);
            container.AddChild(nextTurnButton);

            return container;
        }

        // Utilities

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

                var renderable = _uiService.FindChild<UiComponent>(name);

                if (renderable != null)
                {
                    renderable.IsVisible = false;
                }
            }
        }

        private void ToggleContainer(string name, params string[] except)
        {
            CloseAllContainers(except.Append(name).ToArray());

            var container = _uiService.FindChild<ContainerBase>(name);
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

            _loggingService.Info($"Game ID: {_dataService.CurrentGame.GameId}");
            _loggingService.Info($"Person count: {_dataService.CurrentGame.People.Count}");
            _loggingService.Info($"Company count: {_dataService.CurrentGame.Companies.Count}");
            _loggingService.Info($"Project count: {_dataService.CurrentGame.Projects.Count}");

            // Maybe make Companies observable?
            foreach (var company in _dataService.CurrentGame.Companies)
            {
                var founder = company.Founder.Person;
                _loggingService.Info($"Company {company.Id}: {company.Name} founded by {founder.FirstName} {founder.LastName}");
            }

            (sender as Button).IsEnabled = false;
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

                        var skill = allocation.Employee.Person.Skills.FirstOrDefault(s => s.SkillId == requirement.SkillId); ;

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

                var currentProjects = company.Projects.ToList();

                foreach (var project in currentProjects)
                {
                    var isComplete = project.Requirements.All(r => r.CurrentAmount >= r.TargetAmount);

                    if (isComplete)
                    {
                        company.Money += project.Value;
                        project.Status = ProjectStatus.Completed;

                        var toRemove = company.Allocations.Where(a => a.Project == project).ToList();

                        foreach (var allocation in toRemove)
                        {
                            company.Allocations.Remove(allocation);
                        }

                        company.ArchivedProjects.Add(project);
                        company.Projects.Remove(project);

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

                        company.ArchivedProjects.Add(project);
                        company.Projects.Remove(project);

                        continue;
                    }
                }
            }

            _dataService.CurrentGame.Turn++;
        }

        // Projects

        private void OnProjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var projectContainer = _uiService.FindChild<ContainerBase>("container-projects");

            if (e.OldItems != null)
            {
                foreach (Project project in e.OldItems)
                {
                    _loggingService.Info($"Removing project {project.Id}: {project.Name}");
                    RemoveProject(project, projectContainer);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Project project in e.NewItems)
                {
                    _loggingService.Info($"Adding project {project.Id}: {project.Name}");
                    AddProject(project, projectContainer);
                }
            }
        }

        private void AddProject(Project project, IContainer container)
        {
            var button = _uiService.CreateButton(project.Name, ToggleProject, new DataEventArgs<Project>(project));
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
            var container = _uiService.FindChild<ContainerBase>("container-people");

            if (e.OldItems != null)
            {
                foreach (Person person in e.OldItems)
                {
                    _loggingService.Info($"Removing person {person.Id}: {person.FullName}");
                    RemovePerson(person, container);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Person person in e.NewItems)
                {
                    _loggingService.Info($"Adding person {person.Id}: {person.FullName}");
                    AddPerson(person, container);
                }
            }
        }

        private void AddPerson(Person person, IContainer container)
        {
            _loggingService.Info($"Adding person {person.Id}: {person.FullName}");
            var button = _uiService.CreateButton($"{person.FullName}", TogglePerson, new DataEventArgs<Person>(person));
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
            var previousContainer = _uiService.FindChild<ContainerBase>("container-person-details");
            var previousPerson = _resourceService.Get<Person>("previous-person");

            _uiService.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousPerson)
            {
                _resourceService.Set("previous-person", null);
                return;
            }

            _resourceService.Set("previous-person", args.Data);

            var currentContainer = CreatePersonDetailsContainer(args.Data);

            _uiService.AddChild(currentContainer);
        }

        private IRenderable CreatePeopleContainer()
        {
            return new StackContainer
            {
                Position = new Vector2(0, 100),
                Padding = new Vector4(20, 5, 20, 5),
                Locator = "container-people"
            };
        }

        private IRenderable CreatePersonDetailsContainer(Person person)
        {
            var root = new StackContainer
            {
                Position = new Vector2(400, 100),
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-person-details"
            };

            var font = _resourceService.Get<SpriteFont>("font-default");

            // Name
            var nameLabel = new LabeledTextBox
            {
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
                Font = font,
                Color = Color.Black,
                Label = "Gender",
                LabelWidth = 75,
                Text = $"{person.Gender}"
            };
            root.AddChild(genderLabel);

            // Hire Button
            var hireButton = _uiService.CreateButton("Hire", (s, e) => HirePerson(person));
            hireButton.Locator = $"person-hire-button-{person.Id}";
            root.AddChild(hireButton);

            // Skills
            var skillsLabel = new TextBox
            {
                Font = font,
                Color = Color.Black,
                Text = "Skills"
            };
            root.AddChild(skillsLabel);

            var skillsContainer = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };
            root.AddChild(skillsContainer);

            foreach (var personSkill in person.Skills.OrderByDescending(s => s.Value))
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == personSkill.SkillId);

                var skillLabel = new LabeledTextBox
                {
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

            var details = _uiService.FindChild<UiComponent>("container-person-details");
            details.IsVisible = false;
        }

        // Projects

        private void ToggleProjectsList(object sender, EventArgs e) => ToggleContainer("container-projects");

        private void ToggleProject(object sender, DataEventArgs<Project> args)
        {
            var previousContainer = _uiService.FindChild<ContainerBase>("container-project-details");

            var previousProject = _resourceService.Get<Project>("previous-project");

            _uiService.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousProject)
            {
                _resourceService.Set("previous-project", null);
                return;
            }

            _resourceService.Set("previous-project", args.Data);

            var currentContainer = CreateProjectDetailsContainer(args.Data);

            _uiService.AddChild(currentContainer);
        }

        private IRenderable CreateProjectsContainer()
        {
            var container = new StackContainer
            {
                Position = new Vector2(0, 100),
                Padding = new Vector4(20, 5, 20, 5),
                Locator = "container-projects"
            };

            return container;
        }

        private IContainer CreateProjectDetailsContainer(Project project)
        {
            var root = new StackContainer
            {
                Position = new Vector2(400, 100),
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-project-details"
            };

            var font = _resourceService.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                Text = project.Name,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            var descriptionLabel = new TextBox
            {
                Text = project.Description,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(descriptionLabel);

            var valueLabel = new LabeledTextBox
            {
                Label = "Pays",
                LabelWidth = 100,
                Text = $"${project.Value:0.00}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(valueLabel);

            var dueDateLabel = new LabeledTextBox
            {
                Label = "Due on turn",
                LabelWidth = 100,
                Text = $"{project.TurnDue}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(dueDateLabel);

            var acceptButton = _uiService.CreateButton("Accept", OnAcceptProject, new DataEventArgs<Project>(project));

            root.AddChild(acceptButton);

            var requirementsLabel = new TextBox
            {
                Font = font,
                Color = Color.Black,
                Text = $"Requirements ({project.Requirements.Sum(r => r.TargetAmount):0.0})"
            };

            root.AddChild(requirementsLabel);

            var requirements = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };

            root.AddChild(requirements);

            foreach (var requirement in project.Requirements)
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == requirement.SkillId);

                var requirementLabel = new LabeledTextBox
                {
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

            var details = _uiService.FindChild<UiComponent>("container-project-details");
            details.IsVisible = false;
        }

        // My Company

        private void ToggleMyCompany(object sender, EventArgs e) => ToggleContainer("container-my-company");

        private IRenderable CreateMyCompanyContainer()
        {
            var myCompanyContainer = new VisualContainer
            {
                Locator = "container-my-company"
            };

            var buttonsContainer = new StackContainer
            {
                Position = new Vector2(0, 100),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };

            myCompanyContainer.AddChild(buttonsContainer);

            var employeesButton = _uiService.CreateButton("Employees", ToggleCompanyEmployees);
            buttonsContainer.AddChild(employeesButton);

            var projectsButton = _uiService.CreateButton("Projects", ToggleCompanyProjects);
            buttonsContainer.AddChild(projectsButton);

            return myCompanyContainer;
        }

        private IContainer CreateMyCompanyProjectsContainer()
        {
            return new StackContainer
            {
                Position = new Vector2(400, 100),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5),
                Locator = "container-my-company-projects"
            };
        }

        private IContainer CreateMyCompanyEmployeesContainer()
        {
            return new StackContainer
            {
                Position = new Vector2(400, 100),
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5),
                Locator = "container-my-company-employees"
            };
        }

        private void ToggleCompanyProjects(object sender, EventArgs e) => ToggleContainer("container-my-company-projects", "container-my-company");

        private void ToggleCompanyEmployees(object sender, EventArgs e) => ToggleContainer("container-my-company-employees", "container-my-company");

        private void ToggleCompanyProject(object sender, DataEventArgs<Project> args)
        {
            var previousContainer = _uiService.FindChild<ILocatable>("container-my-company-project-details");

            var previousProject = _resourceService.Get<Project>("previous-my-company-project");

            _uiService.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousProject)
            {
                _resourceService.Set("previous-my-company-project", null);
                return;
            }

            _resourceService.Set("previous-my-company-project", args.Data);

            var currentContainer = CreateCompanyProjectDetailsContainer(args.Data);

            _uiService.AddChild(currentContainer);
        }

        private void ToggleCompanyEmployee(object sender, DataEventArgs<Employee> args)
        {
            var previousContainer = _uiService.FindChild<ILocatable>("container-my-company-employee-details");

            var previousEmployee = _resourceService.Get<Employee>("previous-my-company-employee");

            _uiService.RemoveChild(previousContainer);

            if (args.Data == null || args.Data == previousEmployee)
            {
                _resourceService.Set("previous-my-company-employee", null);
                return;
            }

            _resourceService.Set("previous-my-company-employee", args.Data);

            var currentContainer = CreateCompanyEmployeeDetailsContainer(args.Data);

            _uiService.AddChild(currentContainer);
        }

        private void OnCompanyProjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var container = _uiService.FindChild<ContainerBase>("container-my-company-projects");

            if (e.OldItems != null)
            {
                foreach (Project project in e.OldItems)
                {
                    _loggingService.Info($"Removing player project {project.Id}: {project.Name}");
                    var child = container.FindChild($"my-company-view-project-{project.Id}");
                    container.RemoveChild(child);
                    var currentProject = _resourceService.Get<Project>("previous-my-company-project");
                    if (currentProject == project)
                    {
                        var projectDetailsContainer = _uiService.FindChild<ContainerBase>("container-my-company-project-details");
                        projectDetailsContainer.Parent.RemoveChild(projectDetailsContainer);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (Project project in e.NewItems)
                {
                    _loggingService.Info($"Adding player project {project.Id}: {project.Name}");
                    var button = _uiService.CreateButton(project.Name, ToggleCompanyProject, new DataEventArgs<Project>(project));
                    button.Locator = $"my-company-view-project-{project.Id}";
                    container.AddChild(button);
                }
            }
        }

        private void OnCompanyEmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var container = _uiService.FindChild<ContainerBase>("container-my-company-employees");

            if (e.OldItems != null)
            {
                foreach (Employee employee in e.OldItems)
                {
                    _loggingService.Info($"Removing player employee {employee.Person.Id}");
                    var child = container.FindChild($"my-company-view-employee-{employee.Person.Id}");
                    container.RemoveChild(child);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Employee employee in e.NewItems)
                {
                    _loggingService.Info($"Adding player employee {employee.Person.Id}");
                    var button = _uiService.CreateButton(employee.Person.FullName, ToggleCompanyEmployee, new DataEventArgs<Employee>(employee));
                    button.Locator = $"my-company-view-employee-{employee.Person.Id}";
                    container.AddChild(button);
                }
            }
        }

        private IContainer CreateCompanyProjectDetailsContainer(Project project)
        {
            var projectContainer = new VisualContainer
            {
                Position = new Vector2(800, 100),
                Locator = "container-my-company-project-details"
            };

            var root = new StackContainer
            {
                Padding = new Vector4(0, 10, 0, 10)
            };

            projectContainer.AddChild(root);

            var font = _resourceService.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                Text = project.Name,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            var descriptionLabel = new TextBox
            {
                Text = project.Description,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(descriptionLabel);

            var valueLabel = new LabeledTextBox
            {
                Label = "Pays",
                LabelWidth = 100,
                Text = $"${project.Value:0.00}",
                Color = Color.Black,
                Font = font
            };

            root.AddChild(valueLabel);

            var dueDateLabel = new LabeledTextBox
            {
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
                var allocationsButton = _uiService.CreateButton("Allocations", ToggleCompanyProjectAllocationsContainer, new DataEventArgs<Project>(project));
                root.AddChild(allocationsButton);
            }

            var requirementsLabel = new TextBox
            {
                Font = font,
                Color = Color.Black,
                Text = $"Requirements ({project.Requirements.Sum(r => r.TargetAmount):0.0})"
            };

            root.AddChild(requirementsLabel);

            var requirements = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };

            root.AddChild(requirements);

            foreach (var requirement in project.Requirements)
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == requirement.SkillId);

                var requirementLabel = new LabeledTextBox
                {
                    Font = font,
                    Color = Color.Black,
                    Position = new Vector2(50, 0),
                    Label = skill.Name,
                    LabelWidth = 75
                };
                requirementLabel.Bind(x => x.Text = $"{requirement.CurrentAmount:0.0} / {requirement.TargetAmount:0.0}");

                requirements.AddChild(requirementLabel);
            }

            return projectContainer;
        }

        private void ToggleCompanyProjectAllocationsContainer(object sender, DataEventArgs<Project> e)
        {
            var projectContainer = _uiService.FindChild<ContainerBase>("container-my-company-project-details");

            var previousContainer = projectContainer.FindChild("container-my-company-project-allocations");
            projectContainer.RemoveChild(previousContainer);

            var previousData = _resourceService.Get<Project>("my-company-project-allocations-current");
            if (previousData == e.Data)
            {
                _resourceService.Set("my-company-project-allocations-current", null);
                return;
            }

            _resourceService.Set("my-company-project-allocations-current", e.Data);

            var container = CreateCompanyProjectAllocationsContainer(e.Data);

            projectContainer.AddChild(container);
        }

        private void OnCompanyAllocationsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var currentProject = _resourceService.Get<Project>("my-company-project-allocations-current");

            var currentProjectAllocationsContainer = _uiService.FindChild<ContainerBase>("container-my-company-project-allocations-current");

            if (args.OldItems != null)
            {
                var currentProjectEmployeesContainer = _uiService.FindChild<ContainerBase>("container-my-company-project-employees");

                foreach (Allocation allocation in args.OldItems)
                {
                    if (allocation.Project != currentProject)
                    {
                        continue;
                    }

                    _loggingService.Info($"Adding company project allocation {allocation.Employee.Person.Id} -> {allocation.Project.Id}");
                    var child = currentProjectAllocationsContainer.FindChild($"project-allocation-{allocation.Employee.Person.Id}");
                    currentProjectAllocationsContainer.RemoveChild(child);

                    currentProjectEmployeesContainer.AddChild(_uiService.CreateButton(allocation.Employee.Person.FullName, (s, e) =>
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

                    _loggingService.Info($"Adding company project allocation {allocation.Employee.Person.Id} -> {allocation.Project.Id}");
                    var uiAllocation = CreateCompanyProjectAllocation(allocation);
                    currentProjectAllocationsContainer.AddChild(uiAllocation);
                }
            }
        }

        private IRenderable CreateCompanyProjectAllocation(Allocation allocation)
        {
            var container = new StackContainer
            {
                Padding = new Vector4(10, 0, 10, 0),
                Direction = LayoutDirection.Horizontal,
                Locator = $"project-allocation-{allocation.Employee.Person.Id}",
            };

            var font = _resourceService.Get<SpriteFont>("font-default");

            container.AddChild(new TextBox
            {
                Text = allocation.Employee.Person.FullName,
                Font = font,
                Color = Color.Black
            });

            var value = new TextBox
            {
                Text = $"{allocation.Percent:P0}",
                Font = font,
                Color = Color.Black
            };

            var minus = _uiService.CreateButton("-", 0, 0, 35, 35, (s, e) =>
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

            var plus = _uiService.CreateButton("+", 0, 0, 35, 35, (s, e) =>
            {
                allocation.Percent += .1f;
                if (allocation.Percent > 1f)
                {
                    allocation.Percent = 1f;
                }
                value.Text = $"{allocation.Percent:P0}";
            });
            container.AddChild(plus);

            var delete = _uiService.CreateButton("X", 0, 0, 35, 35, (s, e) =>
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
                Position = new Vector2(300, 0),
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-my-company-project-allocations"
            };

            var currentContainer = new StackContainer
            {
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
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-my-company-project-employees"
            };

            container.AddChild(employeesContainer);

            var employees = _dataService.CurrentGame.PlayerCompany.Employees.Where(e => !allocations.Any(a => a.Employee == e));

            foreach (var employee in employees)
            {
                employeesContainer.AddChild(_uiService.CreateButton(employee.Person.FullName, (s, e) =>
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
                Position = new Vector2(800, 100),
                Padding = new Vector4(0, 10, 0, 10),
                Locator = "container-my-company-employee-details"
            };

            var font = _resourceService.Get<SpriteFont>("font-default");

            var nameLabel = new TextBox
            {
                Text = employee.Person.FullName,
                Color = Color.Black,
                Font = font
            };
            root.AddChild(nameLabel);

            // Gender
            var genderLabel = new LabeledTextBox
            {
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
                Font = font,
                Color = Color.Black,
                Label = "Salary",
                LabelWidth = 75,
                Text = $"{employee.Salary}"
            };

            root.AddChild(salaryLabel);

            // Fire
            var fireButton = _uiService.CreateButton("Fire", OnFireEmployee, new DataEventArgs<Employee>(employee));
            fireButton.IsEnabled = !employee.IsFounder;
            root.AddChild(fireButton);

            // Skills
            var skillsLabel = new TextBox
            {
                Font = font,
                Color = Color.Black,
                Text = "Skills"
            };

            root.AddChild(skillsLabel);

            var skillsContainer = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 5, 0, 5)
            };
            root.AddChild(skillsContainer);

            foreach (var personSkill in employee.Person.Skills.OrderByDescending(s => s.Value))
            {
                var skill = _dataService.StaticData.Skills.First(s => s.Id == personSkill.SkillId);

                var skillLabel = new LabeledTextBox
                {
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

            _uiService.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _uiService.Render(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}