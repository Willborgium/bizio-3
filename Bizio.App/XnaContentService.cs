using Hyjynx.Core;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;

namespace Hyjynx.App.Xna
{
    public class XnaContentService : IContentService
    {
        public XnaContentService(IServiceProvider serviceProvider)
        {
            _content = new ContentManager(serviceProvider, "Content");
            _loaders = new Dictionary<Type, Func<string, object>>
            {
                { typeof(IFont), GetFont },
                { typeof(ITexture2D), GetTexture2D }
            };
            _types = new Dictionary<string, Type>();
            _areTypesLoaded = false;
        }

        public T Load<T>(string key)
            where T : class
        {
            var requestedType = typeof(T);

            if (requestedType.GetInterfaces().Contains(typeof(IJsonSerializable)) ||
                requestedType.IsArray)
            {
                return GetJsonSerializable<T>(key);
            }

            if (!_loaders.ContainsKey(requestedType))
            {
                return _content.Load<T>(key);
            }

            return _loaders[requestedType](key) as T;
        }

        public object Load(string type, string key)
        {
            if (_genericLoadMethod == null)
            {
                _genericLoadMethod = GetType().GetMethod(nameof(Load), new[] { typeof(string) });

            }

            var targetType = GetTargetType(type);

            if (targetType == null)
            {
                throw new InvalidOperationException($"Cannot load type '{type}'");
            }

            var method = _genericLoadMethod.MakeGenericMethod(targetType);

            return method.Invoke(this, new object[] { key });
        }

        public object Create(string type, object[] parameters)
        {
            var targetType = GetTargetType(type);

            return Activator.CreateInstance(targetType, parameters);
        }

        private Type GetTargetType(string type)
        {
            TryLoadTypes();

            if (_types.ContainsKey(type))
            {
                return _types[type];
            }

            return Type.GetType(type);
        }

        private void TryLoadTypes()
        {
            if (_areTypesLoaded)
            {
                return;
            }

            _areTypesLoaded = true;

            var currentAssembly = Assembly.GetExecutingAssembly();

            var assemblies = new List<Assembly>
            {
                currentAssembly
            };

            foreach (var assemblyName in currentAssembly.GetReferencedAssemblies())
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }

            var types = GetJsonSerializable<IDictionary<string, string>>("content-types");

            foreach ((var typeName, var qualifiedTypeName) in types)
            {
                foreach (var assembly in assemblies)
                {
                    var type = assembly.GetType(qualifiedTypeName);

                    if (type != null)
                    {
                        _types.Add(typeName, type);
                        break;
                    }
                }
            }
        }

        private IFont GetFont(string key)
        {
            var font = _content.Load<SpriteFont>(key);
            return new DynamicFont(font);
        }

        private ITexture2D GetTexture2D(string key)
        {
            var texture = _content.Load<Texture2D>(key);
            return new DynamicTexture2D(texture);
        }

        private static T GetJsonSerializable<T>(string key)
            where T : class
        {
            var path = $"Content\\{key}.json";

            if (!File.Exists(path))
            {
                return null;
            }

            string data;

            using (var reader = new StreamReader(path))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        private MethodInfo _genericLoadMethod;

        private bool _areTypesLoaded;

        private readonly IDictionary<string, Type> _types;

        private readonly IDictionary<Type, Func<string, object>> _loaders;

        private readonly ContentManager _content;
    }
}
