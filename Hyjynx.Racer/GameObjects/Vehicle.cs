using Hyjynx.Core.Rendering;
using System.Numerics;

namespace Hyjynx.Racer.GameObjects
{
    public class Vehicle
    {
        public float Velocity { get; private set; }

        public Func<VehicleInputs> GetInputs { get; set; }

        public Vehicle(Sprite target, IVehicleData vehicleData)
        {
            _target = target;
            _target.Bind(x => Update());
            _vehicleData = vehicleData;
        }

        public void Update()
        {
            var inputs = GetInputs();

            TryMoveCar(inputs);
        }

        private void TryMoveCar(VehicleInputs inputs)
        {
            var acceleration = 0f;
            var rotation = 0f;

            if (inputs.Accelerate)
            {
                acceleration = _vehicleData.Acceleration;
            }
            if (inputs.Brake)
            {
                acceleration = -_vehicleData.Acceleration;
            }

            if (inputs.TurnLeft)
            {
                rotation -= _vehicleData.TurnSpeed;
            }
            if (inputs.TurnRight)
            {
                rotation += _vehicleData.TurnSpeed;
            }

            if (acceleration != 0f)
            {
                Velocity += acceleration;
            }

            if (Velocity > 0.09f && acceleration <= 0)
            {
                Velocity -= _vehicleData.ForwardDeceleration;
            }

            if (Velocity < -0.09f && acceleration >= 0)
            {
                Velocity += _vehicleData.ReverseDeceleration;
            }

            if (acceleration == 0 &&
                Velocity < -0.11f &&
                Velocity > 0.11f)
            {
                Velocity = 0f;
            }

            if (Velocity == 0f)
            {
                return;
            }

            if (Velocity > _vehicleData.TopForwardSpeed)
            {
                Velocity = _vehicleData.TopForwardSpeed;
            }
            else if (Velocity < _vehicleData.TopReverseSpeed)
            {
                Velocity = _vehicleData.TopReverseSpeed;
            }

            float turnSpeedCoefficient;

            if (Velocity < 0)
            {
                turnSpeedCoefficient = Velocity / _vehicleData.TopReverseSpeed;
            }
            else
            {
                turnSpeedCoefficient = Velocity / _vehicleData.TopForwardSpeed;
            }

            if (rotation != 0f)
            {
                if (Velocity > 0f)
                {
                    _target.Rotation += rotation * turnSpeedCoefficient;
                }
                else
                {
                    _target.Rotation -= rotation * turnSpeedCoefficient;
                }
            }

            while (_target.Rotation > TWO_PI)
            {
                _target.Rotation -= TWO_PI;
            }

            while (_target.Rotation < -TWO_PI)
            {
                _target.Rotation += TWO_PI;
            }

            var r = _target.Rotation;

            var x = (float)Math.Cos(r);
            var y = (float)Math.Sin(r);

            var translation = Velocity * new Vector2(x, y);

            _target.Translate(translation);
        }

        private readonly IVehicleData _vehicleData;
        private readonly Sprite _target;

        private const float TWO_PI = (float)Math.PI * 2f;
    }
}
