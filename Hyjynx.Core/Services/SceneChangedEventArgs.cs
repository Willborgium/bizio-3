using System;

namespace Hyjynx.Core.Services
{
    public class SceneChangedEventArgs : EventArgs
    {
        public IScene PreviousScene { get; }
        public IScene CurrentScene { get; }

        public SceneChangedEventArgs(IScene previous, IScene current)
        {
            PreviousScene = previous;
            CurrentScene = current;
        }
    }
}