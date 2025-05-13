using System;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace GameEnvironment
{
    public class ChargerZone: MonoBehaviour
    {
        private static readonly int OutlineFillProperty = Shader.PropertyToID("_OutlineFill");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        
        [SerializeField] private float fillDuration = 1.5f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private RangeDetector rangeDetector;
        
        private Material _material;
        private Tween _currentTween;
        
        private void Start()
        {
            _material = GetComponent<SpriteRenderer>().material;
            _material.SetFloat(OutlineFillProperty, 0);

            rangeDetector.OnPlayerEnterRange += FadeInPlayerEnterRange;
            rangeDetector.OnPlayerExitRange += FadeOutOnPlayerExitRange;
            rangeDetector.OnRangeDestroyed += FadeOutOnPlayerExitRange;
        }
        private void OnDestroy()
        {
            rangeDetector.OnPlayerEnterRange -= FadeInPlayerEnterRange;
            rangeDetector.OnPlayerExitRange -= FadeOutOnPlayerExitRange;
            rangeDetector.OnRangeDestroyed -= FadeOutOnPlayerExitRange;
        }
        public void BeginCharge()
        {
            KillTween(); 
            _currentTween = DOTween.To(
                () => _material.GetFloat(OutlineFillProperty),
                x => _material.SetFloat(OutlineFillProperty, x),
                1f,
                fillDuration
            ).SetEase(Ease.Linear);
        }
        public void CancelCharge()
        {
            KillTween();
            _currentTween = DOTween.To(
                () => _material.GetFloat(OutlineFillProperty),
                x => _material.SetFloat(OutlineFillProperty, x),
                0f,
                fadeDuration
            ).SetEase(Ease.OutQuad);
        }
        public void ForceFill()
        {
            _material.SetFloat(OutlineFillProperty, 1f); 
        }
        private void KillTween()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }
        }
        private void FadeInPlayerEnterRange()
        {
            KillTween(); 
            var startColor = _material.GetColor(OutlineColorProperty);
            _currentTween = _material.DOColor(new Color(startColor.r, startColor.g, startColor.b, 1f),
                OutlineColorProperty, fadeDuration).SetEase(Ease.OutQuad);
        }
        private void FadeOutOnPlayerExitRange()
        {
            KillTween();
            var startColor = _material.GetColor(OutlineColorProperty);
            _currentTween = _material.DOColor(new Color(startColor.r, startColor.g, startColor.b, 0f),
                OutlineColorProperty, fadeDuration).SetEase(Ease.OutQuad);
        }
    }
}