using System;
using System.Linq;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Enemies.EnemyTypes
{
    public class TowerHealth: MonoBehaviour, IEnemyHealth
    {
        [SerializeField] private ShootingTower shootingTower;
        [SerializeField] private int maxHealth;
        [SerializeField] private SpriteRenderer[] towerImages;
        [SerializeField] private Color colorOnHit = Color.grey;
        [SerializeField] private float colorStayDuration = 0.2f;
        
        private bool _canBeHit;
        private int _currentHealth;
        private StatesChanger _statesChanger;
        private CancellationToken _destroyCancellationToken;

        [Inject]
        public void Construct(StatesChanger statesChanger)
        {
            _statesChanger = statesChanger;
        }
        private void Awake()
        {
            _statesChanger.OnStateChanged += SetEnabledOnStateChange;
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _currentHealth = maxHealth;
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= SetEnabledOnStateChange;
        }
        public void TakeDamage(int damage)
        {
            if (!_canBeHit)
            {
                return;
            }
            _currentHealth -= damage;
            ShowTakeDamage(_destroyCancellationToken).Forget();
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                DisableTower();
            }
        }
        private async UniTask ShowTakeDamage(CancellationToken token)
        {
            foreach (var image in towerImages)
            {
                image.color = colorOnHit;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(colorStayDuration), cancellationToken: token);
            foreach (var image in towerImages)
            {
                image.color = Color.white;
            }
        }
        private void SetEnabledOnStateChange(GameStates state)
        {
            if (state == GameStates.Fight)
            {
                _canBeHit = true;
            }
            else
            {
                _canBeHit = false;
            }
        }
        private void DisableTower()
        {
            shootingTower.StopShootingAsync(_destroyCancellationToken).Forget();
            _canBeHit = false;
        }
    }
}