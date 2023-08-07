using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;

namespace Hyjynx.Core.Services
{
    public abstract class Scene : IScene
    {
        public Scene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService)
        {
            _resourceService = resourceService;
            _contentService = contentService;
            _loggingService = loggingService;

            _visualRoot = new VisualContainer();
        }

        public virtual void Update()
        {
            _visualRoot?.Update();
        }

        public void Render(IRenderer renderer)
        {
            renderer.Begin2D();

            _visualRoot?.Render(renderer);

            renderer.End2D();
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
