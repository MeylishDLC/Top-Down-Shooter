using System;
using System.Threading;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    public class AreaAttacker: MonoBehaviour
    {
        [SerializeField] private int attackDamage;
        [SerializeField] private float warningDuration;

        private CancellationTokenSource _cancelAttackCts;

        private void Awake()
        {
            
        }

        //todo moves slowly to the player
        //if player in range, the range is being shown for some time
        //if player doesnt escape the range in N secs => trigger range attack
        //todo shake cam slightly when attack??



    }
}