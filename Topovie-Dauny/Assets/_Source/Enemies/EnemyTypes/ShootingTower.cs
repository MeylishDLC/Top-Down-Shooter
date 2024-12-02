using System;
using System.Threading;
using Bullets.BulletPatterns;
using Bullets.BulletPools;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    public class ShootingTower: MonoBehaviour, IPoolUser
    {
        [SerializeField] private BulletSpawner[] bulletSpawners;
        [SerializeField] private float shootTime;
        [SerializeField] private float delayBetweenShooting;

        [Header("Move Animation")] 
        [SerializeField] private Transform towerJawTransform;
        [SerializeField] private Animator jawAnimator;
        [SerializeField] private float animationSpeed;
        [SerializeField] private float maxAngle;
        [SerializeField] private float minAngle;
        [SerializeField] private float waitTimeOnAngleReached;
        
        private static readonly int Open = Animator.StringToHash("open");
        private static readonly int Close = Animator.StringToHash("close");
        
        private float _remainingTime;
        private CancellationToken _destroyCancellationToken;
        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            SetBulletSpawnersEnabled(false);
            StartAnimatingCycleAsync(_destroyCancellationToken).Forget();
        }

        public void InjectPool(EnemyBulletPool pool)
        {
            foreach (var bulletSpawner in bulletSpawners)
            {
                bulletSpawner.InjectPool(pool);
            }
        }
        private async UniTask StartAnimatingCycleAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await AnimateAsync(token);
                    await UniTask.Yield();
                }
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
        private async UniTask AnimateAsync(CancellationToken token)
        {
            try
            {
                await towerJawTransform.DORotate(new Vector3(0, 0, maxAngle), animationSpeed)
                    .ToUniTask(cancellationToken: token);
            
                SetTowerShooting(true);
                await UniTask.Delay(TimeSpan.FromSeconds(waitTimeOnAngleReached), cancellationToken: token);
                SetTowerShooting(false);
                
                await towerJawTransform.DORotate(new Vector3(0, 0, minAngle), animationSpeed)
                    .ToUniTask(cancellationToken: token);
            
                SetTowerShooting(true);
                await UniTask.Delay(TimeSpan.FromSeconds(waitTimeOnAngleReached),  cancellationToken: token);
                SetTowerShooting(false);
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
        private void SetTowerShooting(bool isShooting)
        {
            if (isShooting)
            {
                jawAnimator.SetTrigger(Open);
                SetBulletSpawnersEnabled(true);
            }
            else
            {
                jawAnimator.SetTrigger(Close);
                SetBulletSpawnersEnabled(false);
            }
        }
        private void SetBulletSpawnersEnabled(bool isEnabled)
        {
            foreach (var spawner in bulletSpawners)
            {
                spawner.enabled = isEnabled;
            }
        }
    }
}