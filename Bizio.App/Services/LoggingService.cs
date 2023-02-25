using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bizio.App.Services
{
    public class LoggingService : ILoggingService
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; } = int.MaxValue;

        public LoggingService()
        {
            _messages = new List<Message>();
        }

        public void Initialize(SpriteFont font, Texture2D pixel)
        {
            _font = font;
            _background = pixel;
            _backgroundDestination = new Rectangle(0, 0, 500, 2000);
        }

        public void Info(string message)
        {
            _messages.Add(new(message, Color.Black));
        }

        public void Warning(string message)
        {
            _messages.Add(new(message, Color.Orange));
        }

        public void Error(string message)
        {
            _messages.Add(new(message, Color.Maroon));
        }

        public void Render(SpriteBatch renderer)
        {
            var offset = _font.LineSpacing * 1.2f;

            var position = new Vector2(5, 5);

            renderer.Draw(_background, _backgroundDestination, Color.White);

            for (var index = _messages.Count - 1; index >= 0; index--)
            {
                var message = _messages[index];
                var timestamp = message.Timestamp.ToString("HH:mm:ss.fff");
                renderer.DrawString(_font, $"{timestamp} {message.Text}", position, message.Color);
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
        private SpriteFont _font;
        private Texture2D _background;
        private readonly List<Message> _messages;
    }
}
