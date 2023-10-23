using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using System.Diagnostics;

namespace Hyjynx.Core.Services
{
    public abstract class Scene : IScene
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public string? Identifier { get; set; }

        public Scene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService)
        {
            _resourceService = resourceService;
            _contentService = contentService;
            _loggingService = loggingService;

            IsVisible = true;
            ZIndex = 0;

            _visualRoot = new VisualContainer();
        }

        public virtual void Update()
        {
            _visualRoot?.Update();
        }

        public void Rasterize(IRenderer renderer)
        {
            //Debug.WriteLine(">>>>> Rasterize start");
            _visualRoot?.Rasterize(renderer);
            //Debug.WriteLine(">>>>> Rasterize end");
        }

        public void Render(IRenderer renderer)
        {
            //Debug.WriteLine(">>>>> Render start");
            _visualRoot?.Render(renderer);
            //Debug.WriteLine(">>>>> Render end");
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void RegisterEvents() { }

        public virtual void UnregisterEvents() { }

        protected IContainer _visualRoot;

        protected readonly IResourceService _resourceService;
        protected readonly IContentService _contentService;
        protected readonly ILoggingService _loggingService;
    }
}
