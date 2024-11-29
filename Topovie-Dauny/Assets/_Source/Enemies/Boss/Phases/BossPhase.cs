using System.Collections.Generic;
using Enemies.Boss.BossAttacks;
using UnityEngine;

namespace Enemies.Boss.Phases
{
    [System.Serializable]
    public class BossPhase
    {
        [BossAttack]
        [SerializeField] private List<MonoBehaviour> bossAttacks;
    }
}