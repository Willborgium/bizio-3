namespace Hyjynx.Core
{
    public interface IDriverImplementation
    {
        event EventHandler Exit;

        void Draw();
        void Initialize();
        void Update();
    }
}