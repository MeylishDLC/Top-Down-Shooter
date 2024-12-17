using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using SoundSystem;
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
        [SerializeField] private EventReference appearSound;
        
        [Header("UI Animation")] 
        [SerializeField] private float scaleValue;
        [SerializeField] private float scaleDuration;

        private Vector3 _initScale;
        private InputListener _inputListener;
        private AudioManager _audioManager;
        
        [Inject]
        public void Construct(InputListener inputListener, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _inputListener = inputListener;
            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(CloseConfirmationScreen);
            _initScale = confirmationPanel.localScale;
        }
        public void OpenConfirmationScreen()
        {
            if (gameObject.activeSelf)
            {
                return;
            }
            if (!appearSound.IsNull)
            {
                _audioManager.PlayOneShot(appearSound);
            }
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
            
            await confirmationPanel.DOScale(scaleValue, scaleDuration).SetLoops(2, LoopType.Yoyo)
                .ToUniTask(cancellationToken: token);
            confirmationPanel.localScale = _initScale;
            
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