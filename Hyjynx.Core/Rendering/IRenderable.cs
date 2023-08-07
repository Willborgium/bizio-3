namespace Hyjynx.Core.Rendering
{
    public interface IRenderable : IIdentifiable
    {
        bool IsVisible { get; set; }

        int ZIndex { get; set; }

        void Render(IRenderer renderer);
    }
}
