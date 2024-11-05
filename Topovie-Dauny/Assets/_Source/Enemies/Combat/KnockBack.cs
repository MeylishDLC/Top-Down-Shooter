using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Combat
{
    public class KnockBack
    {
        public bool GettingKnockedBack { get; private set; }

        private int _knockbackTime;
        private float _knockbackThrust;
        private Rigidbody2D _rb;
        public KnockBack(Rigidbody2D rb, int knockbackTime, float knockbackThrust)
        {
            _rb = rb;
            _knockbackTime = knockbackTime;
            _knockbackThrust = knockbackThrust;
        }

        public void GetKnockedBack(Transform damageSource)
        {
            GettingKnockedBack = true;
            var difference = (_rb.transform.position - damageSource.position).normalized * _knockbackThrust * _rb.mass;
            _rb.AddForce(difference, ForceMode2D.Impulse);
            
            //todo: check smth with the cts
            KnockAsync(CancellationToken.None).Forget();
        }

        private async UniTask KnockAsync(CancellationToken token)
        {
            await UniTask.Delay(_knockbackTime, cancellationToken: token);
            if (_rb is null)
            {
                return;
            }
            _rb.velocity = Vector2.zero;
            GettingKnockedBack = false;
        }
    }
}