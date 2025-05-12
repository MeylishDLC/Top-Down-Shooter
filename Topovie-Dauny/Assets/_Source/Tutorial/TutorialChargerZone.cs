using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(Collider2D))]
    public class TutorialChargerZone: MonoBehaviour
    {
        private static readonly int OutlineFillProperty = Shader.PropertyToID("_OutlineFill");
        public bool IsPlayerInRange {get; private set;}
        
        [SerializeField] private float fillDuration = 0.5f;

        private CancellationToken _ctOnDestroy;
        private Material _material;
        private void Start()
        {
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();
            _material = GetComponent<SpriteRenderer>().material;
            _material.SetFloat(OutlineFillProperty, 0);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            SetPlayerInRange(true, other);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            SetPlayerInRange(true, other);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            SetPlayerInRange(false, other);
        }

        public UniTask Fill(float fillEndValue)
        {
            return DOTween.To(() => _material.GetFloat(OutlineFillProperty),
                    x => _material.SetFloat(OutlineFillProperty, x),
                    fillEndValue, fillDuration).SetEase(Ease.Linear).ToUniTask(cancellationToken: _ctOnDestroy);
        }
        private void SetPlayerInRange(bool entering, Collider2D other)
        {
            if (entering)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    IsPlayerInRange = true;
                }
            }
            else
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    IsPlayerInRange = false;
                }
            }
        }
    }
}