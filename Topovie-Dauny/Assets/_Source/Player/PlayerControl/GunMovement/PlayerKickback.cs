using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player.PlayerControl.GunMovement
{
    public class PlayerKickback
    {
        private Vector3 _targetPosition;
        private Vector3 _initialPosition;
        private float _kickbackStartTime;
        private float _kickbackDistance;
        private float _kickbackDuration;

        private Transform _kickbackTransform;
        private Transform _weaponTransform;
        
        public PlayerKickback(float kickbackDistance, float kickbackDuration, Transform weaponTransform, Transform kickbackTransform)
        {
            _weaponTransform = weaponTransform;
            _kickbackTransform = kickbackTransform;
            
            
            _kickbackDistance = kickbackDistance;
            _kickbackDuration = kickbackDuration;
        }

        public async UniTask ApplyKickback(CancellationToken token)
        {
            var kickbackDirection = -_weaponTransform.right;

            _initialPosition = _kickbackTransform.position;
            _targetPosition = _initialPosition + kickbackDirection * _kickbackDistance;
            _kickbackStartTime = Time.time;

            await HandleKickbackAsync(token);
        }

        private async UniTask HandleKickbackAsync(CancellationToken token)
        {
            var elapsedTime = 0f;

            while (elapsedTime < _kickbackDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / _kickbackDuration;
                _kickbackTransform.position = Vector3.Lerp(_initialPosition, _targetPosition, t);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _kickbackTransform.position = _targetPosition;
        }
    }
}