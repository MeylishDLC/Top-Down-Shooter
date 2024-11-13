using System;
using System.Threading;
using Bullets.BulletPatterns;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies
{
    public class ShootingTower: MonoBehaviour
    {
        [SerializeField] private BulletSpawner bulletSpawner;
        [SerializeField] private float shootTime;
        [SerializeField] private float delayBetweenShooting;
        
        private float _remainingTime;
        private void Start()
        {
            bulletSpawner.enabled = false;
            StartShootingCycle(CancellationToken.None).Forget();
        }
        private async UniTask StartShootingCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await ShootDuringTime(token);
                await DisableShootingForTime(token);
            }
        }
        private async UniTask ShootDuringTime(CancellationToken token)
        {
            bulletSpawner.enabled = true;
            _remainingTime = shootTime;
            while (_remainingTime > 0 || token.IsCancellationRequested)
            {
                _remainingTime -= Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            _remainingTime = 0;
        }

        private async UniTask DisableShootingForTime(CancellationToken token)
        {
            bulletSpawner.enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenShooting), cancellationToken: token);
            bulletSpawner.enabled = true;
        }
    }
}