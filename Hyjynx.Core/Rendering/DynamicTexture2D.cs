namespace Hyjynx.Core.Rendering
{
    public class DynamicTexture2D : ITexture2D
    {
        public DynamicTexture2D(dynamic resource)
        {
            _resource = resource;
        }

        public TResource As<TResource>() => (TResource)_resource;

        protected readonly dynamic _resource;
    }
}
