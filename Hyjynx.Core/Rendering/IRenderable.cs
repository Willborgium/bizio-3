namespace Hyjynx.Core.Rendering
{
    public interface IRenderable : IIdentifiable
    {
        bool IsVisible { get; set; }

        int ZIndex { get; set; }

        void Rasterize(IRenderer renderer);
        void Render(IRenderer renderer);
    }
}
