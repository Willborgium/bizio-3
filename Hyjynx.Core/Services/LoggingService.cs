using Hyjynx.Core.Rendering;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Services
{
    public class LoggingService : ILoggingService
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; } = int.MaxValue;

        public string? Identifier { get; set; }

        public LoggingService()
        {
            _messages = new List<Message>();
        }

        public void Initialize(IFont font, ITexture2D pixel)
        {
            _font = font;
            _background = pixel;
            _backgroundDestination = new Rectangle(0, 0, 500, 2000);
        }

        public ILoggingService Info(string message)
        {
            _messages.Add(new(message, Color.Black));
            return this;
        }

        public ILoggingService Warning(string message)
        {
            _messages.Add(new(message, Color.Orange));
            return this;
        }

        public ILoggingService Error(string message)
        {
            _messages.Add(new(message, Color.Maroon));
            return this;
        }

        public void Rasterize(IRenderer renderer)
        {
        }

        public void Render(IRenderer renderer)
        {
            if (!IsVisible)
            {
                return;
            }

            var offset = _font.LineSpacing * 1.2f;

            var position = new Vector2(5, 5);

            renderer.Draw(_background, _backgroundDestination, Color.White);

            for (var index = _messages.Count - 1; index >= 0; index--)
            {
                var message = _messages[index];
                var timestamp = message.Timestamp.ToString("HH:mm:ss.fff");
                renderer.DrawText(_font, $"{timestamp} {message.Text}", position, message.Color);
                position.Y += offset;
            }
        }

        private struct Message
        {
            public string Text;
            public Color Color;
            public DateTime Timestamp;
            public Message(string text, Color color)
            {
                Text = text;
                Color = color;
                Timestamp = DateTime.Now;
            }
        }

        private Rectangle _backgroundDestination;
        private IFont _font;
        private ITexture2D _background;
        private readonly List<Message> _messages;
    }
}
