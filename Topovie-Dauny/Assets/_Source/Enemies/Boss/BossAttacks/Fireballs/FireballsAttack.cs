using System;
using System.Threading;
using Bullets.BulletPatterns;
using Cysharp.Threading.Tasks;
using SoundSystem;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemies.Boss.BossAttacks.Fireballs
{
    public class FireballsAttack: BulletSpawner, IBossAttack
    {
        [SerializeField] private float minAttackDuration;
        [SerializeField] private float maxAttackDuration;
        [SerializeField] private float extraSoundDuration = 1.5f;

        private CancellationToken _destroyCancellationToken;
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            enabled = false;
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            enabled = true;
            var duration = GetRandomDuration();
            AudioManager.PlayMusicDuringTime(duration + extraSoundDuration, AudioManager.FMODEvents.FireballSound);
            return UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _destroyCancellationToken)
                .ContinueWith(() => enabled = false);
        }

        private float GetRandomDuration()
        {
            return Random.Range(minAttackDuration, maxAttackDuration);
        }
    }
}