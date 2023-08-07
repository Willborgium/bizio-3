namespace Hyjynx.Core.Services
{
    public interface IContentService
    {
        T Load<T>(string key)
            where T : class;

        object Load(string type, string key);

        object Create(string type, object[] parameters);
    }
}
