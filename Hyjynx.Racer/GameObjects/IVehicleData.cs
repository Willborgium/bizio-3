namespace Hyjynx.Racer.GameObjects
{
    public interface IVehicleData
    {
        public float Acceleration { get; }

        public float TopForwardSpeed { get; }
        public float TopReverseSpeed { get; }

        public float ForwardDeceleration { get; }
        public float ReverseDeceleration { get; }

        public float TurnSpeed { get; }
    }
}
