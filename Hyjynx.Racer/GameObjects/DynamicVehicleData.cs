namespace Hyjynx.Racer.GameObjects
{
    public class DynamicVehicleData : IVehicleData
    {
        private readonly Func<float> _accelerationFunc;
        private readonly Func<float> _topForwardSpeedFunc;
        private readonly Func<float> _topReverseSpeedFactorFunc;
        private readonly Func<float> _decelerationFactorFunc;
        private readonly Func<float> _turnSpeedFunc;

        public float Acceleration => _accelerationFunc();

        public float TopForwardSpeed => _topForwardSpeedFunc();
        public float TopReverseSpeed => TopForwardSpeed * -_topReverseSpeedFactorFunc();

        public float ForwardDeceleration => _decelerationFactorFunc() * Acceleration;
        public float ReverseDeceleration => _decelerationFactorFunc() * 1.5f * Acceleration;

        public float TurnSpeed => _turnSpeedFunc();

        public DynamicVehicleData(
            Func<float> acceleration,
            Func<float> topSpeed,
            Func<float> topReverseSpeedFactor,
            Func<float> decelerationFactor,
            Func<float> turnSpeed
            )
        {
            _accelerationFunc = acceleration;
            _topForwardSpeedFunc = topSpeed;
            _topReverseSpeedFactorFunc = topReverseSpeedFactor;
            _decelerationFactorFunc = decelerationFactor;
            _turnSpeedFunc = turnSpeed;
        }
    }
}
