﻿using Hyjynx.Core;

namespace Hyjynx.Bizio.Game
{
    public class StaticData
    {
        public IntRange NewGamePeopleBatchSize { get; set; }
        public IntRange NewPersonSkillCount { get; set; }
        public IntRange NewGameCompanyBatchSize { get; set; }
        public IntRange NewGameProjectBatchSize { get; set; }
        public IDictionary<Gender, IEnumerable<string>> FirstNames { get; set; }
        public IEnumerable<string> LastNames { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public ICollection<string> CompanyNames { get; set; }
    }
}
