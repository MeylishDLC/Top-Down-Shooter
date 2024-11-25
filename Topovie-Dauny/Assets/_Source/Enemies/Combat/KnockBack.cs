using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Combat
{
    public class KnockBack
    {
        public bool GettingKnockedBack { get; private set; }

        private readonly int _knockbackTime;
        private readonly float _knockbackThrust;
        private readonly Rigidbody2D _rb;
        private readonly CancellationToken _destroyCancellationToken;
        public KnockBack(MonoBehaviour objectToKnockback,Rigidbody2D rb, int knockbackTime, float knockbackThrust)
        {
            _rb = rb;
            _knockbackTime = knockbackTime;
            _knockbackThrust = knockbackThrust;
            _destroyCancellationToken = objectToKnockback.GetCancellationTokenOnDestroy();
        }

        public void GetKnockedBack(Transform damageSource)
        {
            GettingKnockedBack = true;
            var difference = (_rb.transform.position - damageSource.position).normalized * _knockbackThrust * _rb.mass;
            _rb.AddForce(difference, ForceMode2D.Impulse);
            
            KnockAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask KnockAsync(CancellationToken token)
        {
            await UniTask.Delay(_knockbackTime, cancellationToken: token);
            _rb.velocity = Vector2.zero;
            GettingKnockedBack = false;
        }
    }
}