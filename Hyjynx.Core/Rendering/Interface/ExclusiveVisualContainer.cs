using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
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
