namespace Hyjynx.Core.Services
{
    public interface IResourceService
    {
        T? Get<T>(string key);
        void Set(string key, object? resource);
        void Set<T>(string key, T? resource);
    }
}