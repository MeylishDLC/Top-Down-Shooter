using System;
using System.Collections.Generic;
using System.Linq;
using Enemies.Boss.BossAttacks;
using UnityEngine;

namespace Enemies.Boss.Phases
{
    public class BossPhase: MonoBehaviour
    { 
        public enum PhaseType
        {
            SingleAttackPhase,
            AttackCombinationPhase
        }
        public IBossPhase BossPhaseInstance {get; private set;}

        [SerializeField] private PhaseType phaseType;
        [SerializeField] private BasePhaseConfig config;
        
        
        [SerializeField] private int a;
        [SerializeField] private int b;
        
        [BossAttack]
        [SerializeField] private List<MonoBehaviour> bossAttacks;
        private void Awake()
        {
            if (phaseType == PhaseType.SingleAttackPhase)
            {
                BossPhaseInstance = new SingleAttackPhase(config, GetCastedBossAttacksList());
            }
        }

        private List<IBossAttack> GetCastedBossAttacksList()
        {
            return bossAttacks.Select(x => x.GetComponent<IBossAttack>()).ToList();
        }
    }
}