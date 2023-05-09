using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Core.Arena
{
    [Serializable]
    public struct ArenaWaveDescription
    {
        public int walkingEnemiesCount;
        public List<EnemySpawnPoint> walkingEnemiesSpawnPoints;
        public int flyeingEnemiesCount;
        public List<EnemySpawnPoint> flyingEnemiesSpawnPoints;
        public int tankEnemiesCount;
        public List<EnemySpawnPoint> tankEnemiesSpawnPoints;
    }
}
