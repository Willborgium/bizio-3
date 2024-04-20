using Hyjynx.Battler.Model;
using Hyjynx.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyjynx.Battler.Services
{
    internal class DevelopmentService : IDevelopmentService
    {
        private readonly IEnumerable<BattlerAttackData> _attacks = new[]
        {
            new BattlerAttackData
            {
                Name = "Kick",
                Power = 5,
                MaxPowerPoints = 20,
                PowerPoints = 20
            },
            new BattlerAttackData
            {
                Name = "Drop Kick",
                Power = 8,
                MaxPowerPoints = 10,
                PowerPoints = 10
            },
            new BattlerAttackData
            {
                Name = "Punch",
                Power = 10,
                MaxPowerPoints = 10,
                PowerPoints = 10
            },
            new BattlerAttackData
            {
                Name = "Slash",
                Power = 15,
                MaxPowerPoints = 5,
                PowerPoints = 5
            }
        };

        private readonly IEnumerable<BattlerToolData> _tools = new[]
        {
            new BattlerToolData
            {
                Name = "X Attack",
                Count = 3
            },
            new BattlerToolData
            {
                Name = "X Defense",
                Count = 3
            },
            new BattlerToolData
            {
                Name = "X Speed",
                Count = 3
            },
            new BattlerToolData
            {
                Name = "X HP",
                Count = 3
            }
        };

        private ICollection<BattlerAttackData> GenerateRandomAttacks(int count, byte? powerPoints = null)
        {
            var output = new List<BattlerAttackData>();

            for (var index = 0; index < count; index++)
            {
                var attack = _attacks.Random(a => !output.Any(o => o.Name == a.Name));

                output.Add(new BattlerAttackData
                {
                    Name = attack.Name,
                    Power = attack.Power,
                    MaxPowerPoints = attack.MaxPowerPoints,
                    PowerPoints = powerPoints ?? attack.PowerPoints
                });
            }

            return output;
        }

        private ICollection<BattlerToolData> GenerateRandomTools(int distinctToolCount, byte? countPerTool = null)
        {
            var output = new List<BattlerToolData>();

            for (var index = 0; index < distinctToolCount; index++)
            {
                var tool = _tools.Random(a => !output.Any(o => o.Name == a.Name));

                output.Add(new BattlerToolData
                {
                    Name = tool.Name,
                    Count = countPerTool ?? tool.Count
                });
            }

            return output;
        }

        public BattleData GenerateBattleData()
        {
            var b1 = new BattlerData
            {
                Id = Guid.NewGuid(),
                Name = "Player Battler",
                Health = (short)Random.Shared.Next(50, 100),
                Attacks = GenerateRandomAttacks(2),
                Tools = GenerateRandomTools(2)
            };

            var b2 = new BattlerData
            {
                Id = Guid.NewGuid(),
                Name = "AI Battler",
                Health = (short)Random.Shared.Next(50, 100),
                Attacks = GenerateRandomAttacks(2, 2),
                Tools = GenerateRandomTools(2)
            };

            return new BattleData
            {
                PlayerBattler = b1,
                ComputerBattler = b2,
            };
        }
    }
}
