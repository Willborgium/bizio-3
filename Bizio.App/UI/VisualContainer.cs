using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Bizio.App.UI
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Previous { get; }

        public T Current { get; }

        public ValueChangedEventArgs(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }
    }

    public class VisualContainer : ContainerBase
    {
        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            return GetCurrentPosition() + child.Position;
        }
    }

    public class Observer<T> : IUpdateable
        where T : struct
    {
        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        public Observer(Func<T> subject)
        {
            _subject = subject;
            _value = default;
        }

        public void Update()
        {
            var currentValue = _subject();

            if(EqualityComparer<T>.Default.Equals(currentValue, _value))
            {
                return;
            }
            
            ValueChanged?.Invoke(this, new(_value, currentValue));

            _value = currentValue;
        }

        private readonly Func<T> _subject;

        private T _value;
    }

    public class ExclusiveVisualContainer : ContainerBase
    {
        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            return GetCurrentPosition() + child.Position;
        }

        protected override void OnChildAdded(IIdentifiable child)
        {
            if (child is IRenderable renderable)
            {
                var observer = new Observer<bool>(() => renderable.IsVisible);

                observer.ValueChanged += (s, e) =>
                {
                    if (!e.Current)
                    {
                        return;
                    }

                    foreach (var sibling in _renderables)
                    {
                        if (sibling == child)
                        {
                            continue;
                        }

                        sibling.IsVisible = false;
                    }
                };

                _observers.Add(child, observer);
                _updateables.Add(observer);
            }

            foreach (var sibling in _renderables)
            {
                if (sibling == child)
                {
                    continue;
                }
            }
        }

        protected override void OnChildRemoved(IIdentifiable child)
        {
            _updateables.Remove(_observers[child]);
            _observers.Remove(child);
        }

        private readonly IDictionary<object, Observer<bool>> _observers = new Dictionary<object, Observer<bool>>();
    }
}
