
namespace Hyjynx.Battler.Model
{
    internal class BattleData
    {
        public BattlerData PlayerBattler { get; set; }
        public BattlerData ComputerBattler { get; set; }
    }

    internal class BattlerData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public short Health { get; set; }

        public ICollection<BattlerAttackData> Attacks { get; set; }
    }

    internal class BattlerAttackData
    {
        public string Name { get; set; }

        public byte Power { get; set; }

        public byte MaxPowerPoints { get; set; }

        public byte PowerPoints { get; set; }
    }
}
