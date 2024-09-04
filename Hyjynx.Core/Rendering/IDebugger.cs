namespace Hyjynx.Core.Rendering
{
    public interface IDebugger : IRenderable
    {
        bool IsEnabled { get; set; }

        void AddLabel(string label, Func<string> valueGetter);
        void BindTo<T>(T target) where T : ITranslatable, IBindable;
    }
}
