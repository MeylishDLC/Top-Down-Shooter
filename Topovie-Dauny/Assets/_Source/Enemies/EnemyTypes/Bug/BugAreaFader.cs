using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ink.Parsed;
using UnityEngine;

namespace Enemies.EnemyTypes.Bug
{
    public class BugAreaFader
    {
        private static readonly int MainColorProperty = Shader.PropertyToID("_Color");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        
        private readonly Color _baseColor;
        private readonly Color _attackColor;

        private readonly float _colorTransitionTime;
        private readonly float _areaFadeTime;

        private readonly float _baseAreaAlpha;
        
        private Material _material;
        
        public BugAreaFader(BugAreaFaderConfig config)
        {
            _baseColor = config.BaseColor;
            _attackColor = config.AttackColor;
            
            _colorTransitionTime = config.ColorTransitionTime;
            _areaFadeTime = config.AreaFadeTime;
            _baseAreaAlpha = config.BaseAreaAlpha;
        }

        public void SetupFader(SpriteRenderer renderer)
        {
            _material = renderer.material;
        }

        public async UniTask FadeAreaAsync(FadeType fadeType, CancellationToken token)
        {
            if (!_material)
            {
                throw new Exception("Material is null. Initialize it through SetupFader");
            }
            switch (fadeType)
            {
                case FadeType.FadeIn:
                    await FadeInAreaAsync(token);
                    break;
                case FadeType.FadeOut:
                    await FadeOutAreaAsync(token);
                    break;
                default:
                    throw new Exception("Unknown FadeType");
            }
        }

        public async UniTask ChangeAreaColorAsync(ColorChangeType changeType, CancellationToken token)
        {
            if (_material == null)
            {
                throw new Exception("Material is null. Initialize it through SetupFader");
            }
            switch (changeType)
            {
                case ColorChangeType.ChangeToBaseColor:
                    await ChangeColorAsync(MainColorProperty, _baseColor, token);
                    break;
                case ColorChangeType.ChangeToAttackColor:
                    await ChangeColorAsync(MainColorProperty, _attackColor, token);
                    break;
                default:
                    throw new Exception("Unknown ColorChangeType");
            }
        }

        private async UniTask FadeInAreaAsync(CancellationToken token)
        {
            var tasks = new List<UniTask>()
            {
                FadeColorAsync(MainColorProperty, _baseAreaAlpha, token),
                FadeColorAsync(OutlineColorProperty, 1f, token)
            };
            await UniTask.WhenAll(tasks);
        }
        private async UniTask FadeOutAreaAsync(CancellationToken token)
        {
            var tasks = new List<UniTask>()
            {
                FadeColorAsync(MainColorProperty, 0f, token),
                FadeColorAsync(OutlineColorProperty, 0f, token),
            };
            await UniTask.WhenAll(tasks);
        }

        private async UniTask FadeColorAsync(int propertyID, float fadeValue, CancellationToken token)
        {
            var startColor = _material.GetColor(propertyID);
            
            await _material.DOColor(new Color(startColor.r, startColor.g, startColor.b, fadeValue),
                propertyID, _areaFadeTime).ToUniTask(cancellationToken: token);

            var currentColor = _material.GetColor(propertyID);
            currentColor.a = 0f;
        }

        private async UniTask ChangeColorAsync(int propertyID, Color color, CancellationToken token)
        {
            var startColor = _material.GetColor(propertyID);

            await _material.DOColor(new Color(color.r, color.g, color.b, startColor.a),
                propertyID, _colorTransitionTime).ToUniTask(cancellationToken: token);
            
            _material.SetColor(propertyID, color);
        }
    }

    public enum FadeType
    {
        FadeIn,
        FadeOut,
    }

    public enum ColorChangeType
    {
        ChangeToBaseColor,
        ChangeToAttackColor
    }
}