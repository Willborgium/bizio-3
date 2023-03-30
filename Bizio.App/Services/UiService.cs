using Bizio.App.UI;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Bizio.App.Services
{
    public class UiService : VisualContainer, IUiService
    {
        public UiService(
            IResourceService resourceService,
            ILoggingService loggingService)
        {
            _resourceService = resourceService;
            _loggingService = loggingService;
            IsVisible = true;
            this.Identifier = "root";
        }

        public void Describe()
        {
            var description = new StringBuilder();

            Describe(description, 0, this);

            var result = description.ToString();

            var filename = $"visual-tree-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffff}.txt";

            using (var writer = new StreamWriter(filename))
            {
                writer.Write(result);
            }

            _loggingService.Info($"Saved visual tree to file {filename}");
        }

        private void Describe(StringBuilder description, int depth, IIdentifiable node)
        {
            var indentation = new string(Enumerable.Repeat('\t', depth).ToArray());
            var id = Identify(node);
            var message = $"{indentation}{id}";
            
            description.AppendLine(message);
            //_loggingService.Info(message);
            
            if (node is IContainer container)
            {
                foreach (var child in container)
                {
                    Describe(description, depth + 1, child);
                }
            }
        }

        private string Identify(IIdentifiable target)
        {
            if (!string.IsNullOrWhiteSpace(target.Identifier))
            {
                return target.Identifier;
            }

            var type = target.GetType().Name.ToString().ToLower();

            var description = "unknown";

            if (target is Button button)
            {
                description = button.Text ?? description;
            }
            else if (target is TextBox textBox)
            {
                description = textBox.Text ?? description;
            }
            else if (target is LabeledTextBox labeledTextBox)
            {
                description = labeledTextBox.Label ?? description;
            }

            description = description.Replace(" ", "-").ToLower();

            return $"{type}-{description}-{Guid.NewGuid()}";
        }

        public Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null)
        {
            var metadata = _resourceService.Get<ButtonMetadata>("button-metadata-default");

            var button = new Button(metadata)
            {
                Text = text,
                Position = new Vector2(x, y),
                Args = args ?? EventArgs.Empty
            };
            button.SetDimensions(new Vector2(width, height));

            button.Clicked += handler;

            return button;
        }

        public Button CreateButton<T>(string text, EventHandler<T> handler, T args = null)
            where T : EventArgs
        {
            return CreateButton(text, 0, 0, 300, 50, (s, e) => handler?.Invoke(s, e as T), args);
        }

        public Button CreateButton(string text, EventHandler handler, EventArgs args = null)
        {
            return CreateButton(text, 0, 0, 300, 50, handler, args);
        }

        private readonly IResourceService _resourceService;
        private readonly ILoggingService _loggingService;
    }
}
