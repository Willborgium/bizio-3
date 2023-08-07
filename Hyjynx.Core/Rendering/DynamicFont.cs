using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class DynamicFont : IFont
    {
        public float LineSpacing => _resource.LineSpacing;

        public Vector2 MeasureString(string text)
        {
            var result = _resource.MeasureString(text);

            return new Vector2(result.X, result.Y);
        }

        public DynamicFont(dynamic resource)
        {
            _resource = resource;
        }

        public T As<T>() => (T)_resource;

        private readonly dynamic _resource;
    }
}
