using System;

namespace Bizio.App.Game
{
    public class PersonSkill
    {
        public Guid SkillId { get; set; }
        public float Value { get; set; }
        public PersonSkillLearnRate LearnRate { get; set; }
    }
}
