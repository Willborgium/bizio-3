using System;

namespace Bizio.App.UI
{
    public static class Binding
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

        public static T Bind<T>(this T source, Action<T> bind) where T : UiComponent
        {
            source.Bindings.Add(Create(() => bind(source)));

            return source;
        }
    }

    public interface IBinding : IUpdateable
    {
    }

    public class SimpleBinding : IBinding
    {
        public event Action Bind;

        public void Update()
        {
            Bind?.Invoke();
        }
    }

    public class Binding<TValue> : IBinding
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
