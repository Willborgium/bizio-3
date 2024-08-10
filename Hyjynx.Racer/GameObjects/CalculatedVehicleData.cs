namespace Hyjynx.Racer.GameObjects
{
    public class CalculatedVehicleData : IVehicleData
    {
        public float Acceleration { get; }

        public float TopForwardSpeed { get; }
        public float TopReverseSpeed { get; }

        public float ForwardDeceleration { get; }
        public float ReverseDeceleration { get; }

        public float TurnSpeed { get; }

        public CalculatedVehicleData(
            float acceleration,
            float topSpeed,
            float topReverseSpeedFactor,
            float decelerationFactor,
            float turnSpeed
            )
        {
            Acceleration = acceleration;

            TopForwardSpeed = topSpeed;
            TopReverseSpeed = TopForwardSpeed * -topReverseSpeedFactor;

            ForwardDeceleration = decelerationFactor * Acceleration;
            ReverseDeceleration = decelerationFactor * 1.5f * Acceleration;

            TurnSpeed = turnSpeed;
        }
    }
}
