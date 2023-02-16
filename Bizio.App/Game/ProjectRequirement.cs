using System;

namespace Bizio.App.Game
{
    public class ProjectRequirement
    {
        public Guid SkillId { get; set; }
        public float CurrentAmount { get; set; }
        public float TargetAmount { get; set; }
    }
}
