namespace Hyjynx.Core.Services
{
    public interface IWindowManagementService
    {
        void Initialize(object caller);
        void SetWindowDimensions(int width, int height);
    }
}
