using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies;
using Pathfinding;
using UnityEngine;

namespace Weapons.AbilityWeapons
{
    public class StunZone: MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private float stunDuration;

        private List<AIPath> _enemiesAffected = new();
        private Collider2D _collider;
        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            UnStunEnemiesAfterDuration(CancellationToken.None).Forget();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (other.gameObject.transform.parent.TryGetComponent(out AIPath aiPath))
                {
                    aiPath.canMove = false;
                    var enemyHealth = other.GetComponent<IEnemyHealth>();
                    enemyHealth.TakeDamage(damage);

                    if (!_enemiesAffected.Contains(aiPath))
                    {
                        _enemiesAffected.Add(aiPath);
                    }
                }
            }
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (other.gameObject.transform.parent.TryGetComponent(out AIPath aiPath))
                {
                    aiPath.canMove = false;
                    
                    if (!_enemiesAffected.Contains(aiPath))
                    {
                        _enemiesAffected.Add(aiPath);
                    }
                }
            }
        }
        private async UniTask UnStunEnemiesAfterDuration(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(stunDuration), cancellationToken: token);

            _collider.enabled = false;
            foreach (var enemy in _enemiesAffected)
            {
                if (enemy != null)
                {
                    enemy.canMove = true;
                }
            }
            Destroy(gameObject);
        }
    }
}