using System;
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

        [SerializeField] private AIPath aiPathComponent;
        
        private int _currentHealth;
        private bool _isDead;
        private void Start()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (!_isDead)
            {
                _currentHealth -= damage;
                Debug.Log(_currentHealth);
                CheckIfDeadAsync().Forget();
            }
        }

        private async UniTask CheckIfDeadAsync()
        {
            if (_currentHealth <= 0)
            {
                _isDead = true;
                aiPathComponent.enabled = false;
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