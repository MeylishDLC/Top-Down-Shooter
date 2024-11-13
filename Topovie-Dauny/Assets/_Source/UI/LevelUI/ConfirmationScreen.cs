using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.LevelUI
{
    public class ConfirmationScreen: MonoBehaviour
    {
        public event Action OnConfirmed;
        [SerializeField] private RectTransform confirmationScreen;
        [SerializeField] private RectTransform confirmationPanel;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;

        [Header("UI Animation")] 
        [SerializeField] private float scaleValue;
        [SerializeField] private float scaleDuration;

        private InputListener _inputListener;
        
        [Inject]
        public void Construct(InputListener inputListener)
        {
            _inputListener = inputListener;
            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(CloseConfirmationScreen);
        }
        public void OpenConfirmationScreen()
        {
            _inputListener.SetInput(false);
            confirmationScreen.gameObject.SetActive(true);
            AnimateAsync(CancellationToken.None).Forget();
        }
        private void CloseConfirmationScreen()
        {
            CloseConfirmationScreenAsync(CancellationToken.None).Forget();
        }
        private async UniTask CloseConfirmationScreenAsync(CancellationToken token)
        {
            await AnimateAsync(token);
            confirmationScreen.gameObject.SetActive(false);
            _inputListener.SetInput(true);
        }
        private async UniTask AnimateAsync(CancellationToken token)
        {
            SetButtonActive(false);
            
            var initScale = confirmationPanel.localScale;
            await confirmationPanel.DOScale(scaleValue, scaleDuration).SetLoops(2, LoopType.Yoyo)
                .ToUniTask(cancellationToken: token);
            confirmationPanel.localScale = initScale;
            
            SetButtonActive(true);
        }
        private void Confirm()
        {
            OnConfirmed?.Invoke();
        }
        private void SetButtonActive(bool active)
        {
            if (active)
            {
                cancelButton.interactable = true;
                confirmButton.interactable = true;
            }
            else
            {
                cancelButton.interactable = false;
                confirmButton.interactable = false;
            }
        }
    }
}