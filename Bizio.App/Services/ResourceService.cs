using System.Collections.Generic;

namespace Bizio.App.Services
{
    public class ResourceService
    {
        public T Get<T>(string key)
        {
            if (_resources.ContainsKey(key))
            {
                return (T)_resources[key];
            }

            return default;
        }

        public void Set<T>(string key, T resource)
        {
            _resources[key] = resource;
        }

        public void Set(string key, object resource)
        {
            _resources[key] = resource;
        }

        private readonly IDictionary<string, object> _resources = new Dictionary<string, object>();
    }
}
