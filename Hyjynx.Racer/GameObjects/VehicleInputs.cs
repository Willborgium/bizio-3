namespace Hyjynx.Racer.GameObjects
{
    public record VehicleInputs(bool Accelerate, bool Brake, bool TurnLeft, bool TurnRight)
    {
        public static readonly VehicleInputs None = new(false, false, false, false);
    }
}
