namespace Hyjynx.Core.Services
{
    public interface IKeyStateProvider
    {
        public IReadOnlyDictionary<Keys, KeyState> GetKeyStates();
    }
}
