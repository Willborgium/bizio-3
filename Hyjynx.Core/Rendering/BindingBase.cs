namespace Hyjynx.Core.Rendering
{

    public class BindingBase : IBindable
    {
        public ICollection<IUpdateable> Bindings { get; } = [];

        public virtual void Update()
        {
            foreach (var binding in Bindings)
            {
                binding.Update();
            }
        }
    }
}
