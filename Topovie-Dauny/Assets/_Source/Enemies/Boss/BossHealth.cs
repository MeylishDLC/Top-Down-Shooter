using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies.Boss.Phases;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Enemies.Boss
{
    public class BossHealth: MonoBehaviour, IEnemyHealth
    {
        public event Action OnPhaseFinished;

        [Header("Health Bar Settings")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float ySlideValueOnShow;
        [SerializeField] private float ySlideValueOnHide;
        [SerializeField] private float scaleValueOnBounce;
        [SerializeField] private float yAnimationDuration;
        [SerializeField] private float scaleAnimationDuration;
        
        private int _maxHealth;
        private int _currentHealth;
        
        private RectTransform _sliderRectTransform;
        private BossPhase _currentPhase;
        private bool _isVulnerable;
        private CancellationToken _destroyCancellationToken;

        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _sliderRectTransform = healthSlider.GetComponent<RectTransform>();

            _sliderRectTransform.DOLocalMoveY(ySlideValueOnHide, 0f)
                .ToUniTask(cancellationToken: _destroyCancellationToken);
        }
        public void ChangePhase(BossPhase phase)
        {
            _currentPhase = phase;
            _currentPhase.OnPhaseStateChanged += SetVulnerability;
            
            _maxHealth = _currentPhase.PhaseConfig.BossHealth;
            _currentHealth = _maxHealth;
            UpdateSliderValue();
        }
        public void TakeDamage(int damage)
        {
            if (!_isVulnerable)
            {
                return;
            }
            _currentHealth -= damage;
            UpdateSliderValue();
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                if (_currentPhase != null)
                {
                    OnHealthWasted();
                }
            }
        }
        private void UpdateSliderValue()
        {
            healthSlider.value = _currentHealth / (float)_maxHealth;
        }
        private void OnHealthWasted()
        {
            _currentPhase.FinishPhase();
            _currentPhase.OnPhaseStateChanged -= SetVulnerability;
            _currentPhase = null;
            OnPhaseFinished?.Invoke();
        }
        private void SetVulnerability(PhaseState phaseState)
        {
            if (phaseState == PhaseState.Vulnerability)
            {
                _isVulnerable = true;
                ShowHealthBar(_destroyCancellationToken).Forget();
            }
            else
            {
                _isVulnerable = false;
                HideHealthBar(_destroyCancellationToken).Forget();
            }
        }
        private UniTask ShowHealthBar(CancellationToken token)
        {
            return AnimateSliderYMove(ySlideValueOnShow, token).ContinueWith(() => AnimateSliderScale(token));
        }
        private UniTask HideHealthBar(CancellationToken token)
        {
            return AnimateSliderScale(token).ContinueWith(() => AnimateSliderYMove(ySlideValueOnHide, token));
        }
        private UniTask AnimateSliderYMove(float moveValue, CancellationToken token)
        {
            return _sliderRectTransform.DOLocalMoveY(moveValue, yAnimationDuration)
                .ToUniTask(cancellationToken: token);
        }
        private UniTask AnimateSliderScale(CancellationToken token)
        {
            return _sliderRectTransform.DOScale(scaleValueOnBounce, scaleAnimationDuration).SetLoops(2, LoopType.Yoyo)
                .ToUniTask(cancellationToken: token);
        }
    }
}