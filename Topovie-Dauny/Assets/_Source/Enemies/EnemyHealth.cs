using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace Enemies
{
    public class EnemyHealth: MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int maxMoneyDrop;
        
        [Header("Color Changing")]
        [SerializeField] private Color colorOnDamageTaken;
        [SerializeField] private int stayTimeMilliseconds;

        private AIPath _aiPathComponent;
        private SpriteRenderer _spriteRenderer;
        private int _currentHealth;
        private bool _isDead;
        private void Start()
        {
            _currentHealth = maxHealth;
            _aiPathComponent = GetComponent<AIPath>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void TakeDamage(int damage)
        {
            if (!_isDead)
            {
                _currentHealth -= damage;
                
                //todo: check this one?
                ChangeColor(CancellationToken.None).Forget();
                
                CheckIfDeadAsync(CancellationToken.None).Forget();
            }
        }

        private async UniTask ChangeColor(CancellationToken token)
        {
            _spriteRenderer.color = colorOnDamageTaken;
            await UniTask.Delay(stayTimeMilliseconds, cancellationToken: token);
            _spriteRenderer.color = Color.white;
        }
        private async UniTask CheckIfDeadAsync(CancellationToken token)
        {
            if (_currentHealth <= 0)
            {
                _isDead = true;
                _aiPathComponent.enabled = false;
                await gameObject.transform.DOScaleX(0f, 0.5f).ToUniTask();
                
                DropMoney();
                Destroy(gameObject);
            }
        }

        private void DropMoney()
        {
            //todo: implement dropping money
        }
    }
}