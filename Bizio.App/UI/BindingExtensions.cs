using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Bizio.App.UI
{
    public static class BindingExtensions
    {
        public static IBinding Create<TValue>(Func<TValue> getter, Action<TValue> setter)
        {
            var binding = new Binding<TValue>();

            binding.Getter += getter;
            binding.Setter += setter;

            return binding;
        }

        public static IBinding Create(Action bind)
        {
            var binding = new SimpleBinding();
            binding.Bind += bind;
            return binding;
        }

        public static T Bind<T>(this T source, Action<T> bind)
            where T : UiComponent
        {
            source.Bindings.Add(Create(() => bind(source)));

            return source;
        }

        public static TSource Bind<TSource, TValue>(this TSource source, Func<TValue> getter, Action<TSource, TValue> setter)
            where TSource : UiComponent
        {
            source.Bindings.Add(Create(getter, x => setter(source, x)));
            return source;
        }

        public static TSource BindCollection<TSource, TCollection, TData, TUiComponent>(this TSource source, Func<TData> dataGetter, Func<TData, ObservableCollection<TCollection>> collectionGetter, Func<TCollection, TUiComponent> createUiComponent)
            where TData : class
            where TSource : ContainerBase
            where TUiComponent : IIdentifiable
        {
            void Add(TCollection item)
            {
                var uiComponent = createUiComponent(item);
                uiComponent.Identifier = item.GetHashCode().ToString();
                source.AddChild(uiComponent);
            }

            void Remove(TCollection item)
            {
                var child = source.FindChild(item.GetHashCode().ToString());
                source.RemoveChild(child);
            }

            void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
            {
                if (args.OldItems != null)
                {
                    foreach (TCollection item in args.OldItems)
                    {
                        Remove(item);
                    }
                }

                if (args.NewItems != null)
                {
                    foreach (TCollection item in args.NewItems)
                    {
                        Add(item);
                    }
                }
            }

            source.Bind(x =>
            {
                var previousData = x.GetData<TData>("binding-data-source");

                var currentData = dataGetter();

                if (previousData != currentData)
                {
                    if (previousData != null)
                    {
                        var collection = collectionGetter(previousData);

                        collection.CollectionChanged -= OnCollectionChanged;

                        foreach (var item in collection)
                        {
                            Remove(item);
                        }
                    }

                    if (currentData != null)
                    {
                        var collection = collectionGetter(currentData);
                        collection.CollectionChanged += OnCollectionChanged;

                        foreach (var item in collection)
                        {
                            Add(item);
                        }
                    }

                    x.SetData("binding-data-source", currentData);
                }
            });

            return source;
        }

        private class SimpleBinding : IBinding
        {
            public event Action Bind;

            public void Update()
            {
                Bind?.Invoke();
            }
        }

        private class Binding<TValue> : IBinding
        {
            public event Func<TValue> Getter;

            public event Action<TValue> Setter;

            public void Update()
            {
                if (Getter == null || Setter == null)
                {
                    return;
                }

                Setter(Getter());
            }
        }
    }
}
