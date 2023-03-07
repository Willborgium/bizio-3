using System;

namespace Bizio.App.UI
{
    public static class Binding
    {
        public static Binding<TValue> Create<TValue>(Func<TValue> getter, Action<TValue> setter)
        {
            var binding = new Binding<TValue>();

            binding.Getter += getter;
            binding.Setter += setter;

            return binding;
        }
    }

    public class Binding<TValue> : IUpdateable
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
