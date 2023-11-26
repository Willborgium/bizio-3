
namespace Hyjynx.Battler.Model
{
    internal class BattleData
    {
        public BattlerData PlayerBattler { get; set; }
        public BattlerData ComputerBattler { get; set; }
    }

    internal class BattlerBattleData
    {
        public BattlerData Battler { get; set; }
        public int Health { get; set; }
    }


    internal class BattlerData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Health { get; set; }

        public ICollection<BattlerAttackData> Attacks { get; set; }
    }

    internal class BattlerAttackData
    {
        public string Name { get; set; }

        public int Power { get; set; }
    }
}
