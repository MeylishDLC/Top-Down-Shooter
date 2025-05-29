using System;
using UnityEngine;

namespace GameEnvironment
{
    public class ChargerAnimator: MonoBehaviour
    {
        [SerializeField] private RangeDetector rangeDetector;
        
        private static readonly int ChargeStartProperty = Animator.StringToHash("chargeStart");
        private Animator _animator;
        private PortalChargerTrigger _portalChargerTrigger;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _portalChargerTrigger = rangeDetector.gameObject.GetComponent<PortalChargerTrigger>();
            
            rangeDetector.OnPlayerEnterRange += ResumeAnimation;
            rangeDetector.OnPlayerExitRange += StopAnimation;
            _portalChargerTrigger.OnChargePortalPressed += StartAnimation;
        }
        private void OnDestroy()
        {
            rangeDetector.OnPlayerEnterRange -= ResumeAnimation;
            rangeDetector.OnPlayerExitRange -= StopAnimation;
            _portalChargerTrigger.OnChargePortalPressed -= StartAnimation;
        }
        private void StartAnimation(int _)
        {
            _portalChargerTrigger.OnChargePortalPressed -= StartAnimation;
            _animator.SetTrigger(ChargeStartProperty);
        }
        private void StopAnimation()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && _animator.speed != 0f)
            {
                _animator.speed = 0f;
            }
        }
        private void ResumeAnimation()
        {
            _animator.speed = 1f;
        }
    }
}