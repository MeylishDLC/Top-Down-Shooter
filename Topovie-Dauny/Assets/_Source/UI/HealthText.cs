using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HealthText: MonoBehaviour
    {
        [SerializeField] private Vector3 moveSpeed = new (0, 0.5f, 0);
        [SerializeField] private float timeToFade = 1f;
        
        private TMP_Text _tmpText;
        private RectTransform _textTransform;
        private void Awake()
        {
            _textTransform = GetComponent<RectTransform>();
            _tmpText = GetComponent<TextMeshProUGUI>();
        }
        private void Start()
        {
            Disappear(CancellationToken.None).Forget();
        }

        private async UniTask Disappear(CancellationToken token)
        {
            await _tmpText.DOFade(0f, timeToFade).ToUniTask(cancellationToken: token);
            Destroy(gameObject);
        }
        private void Update()
        {
            _textTransform.position += moveSpeed * Time.deltaTime;
        }
    }
}