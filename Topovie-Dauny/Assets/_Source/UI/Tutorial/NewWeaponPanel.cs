using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using DialogueSystem.LevelDialogue;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Tutorial
{
    public class NewWeaponPanel: MonoBehaviour
    {
        private event Action OnWeaponSwitched;
        
        [SerializeField] private Image panelImage;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float stayDuration = 0.5f;
        [Range(2, 4)]
        [SerializeField] private int newWeaponNumber;

        private bool _newWeaponTaken;
        private LevelDialogues _levelDialogues;
        private DialogueManager _dialogueManager;
        private InputListener _inputListener;
        private CancellationToken _destroyCancellationToken;
        
        [Inject]
        public void Construct(InputListener inputListener, LevelDialogues levelDialogues, DialogueManager dialogueManager)
        {
            _inputListener = inputListener;
            _dialogueManager = dialogueManager;
            _levelDialogues = levelDialogues;
        }
        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            panelImage.gameObject.SetActive(false);
            panelImage.DOFade(0f, 0f);
            _inputListener.OnSwitchWeaponPressed += CheckForWeaponSwitched;
            
            if (_levelDialogues.HasStartDialogue())
            {
                _dialogueManager.OnDialogueEnded += ShowPanel;
            }
            else
            {
                ShowPanel();
            }
        }
        private void ShowPanel()
        {
            _dialogueManager.OnDialogueEnded -= ShowPanel;
            ShowPanelAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask ShowPanelAsync(CancellationToken token)
        {
            panelImage.gameObject.SetActive(true);
            await panelImage.DOFade(1f, fadeDuration).ToUniTask(cancellationToken: token);
            await UniTask.Delay(TimeSpan.FromSeconds(stayDuration), cancellationToken: token);
            if (_newWeaponTaken)
            {
                await HidePanelAsync(token);
            }
            else
            {
                OnWeaponSwitched += HidePanel;
            }
        }
        private void HidePanel()
        {
            OnWeaponSwitched -= HidePanel;
            HidePanelAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask HidePanelAsync(CancellationToken token)
        {
            await panelImage.DOFade(0f, fadeDuration).ToUniTask(cancellationToken: token);
            panelImage.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        private void CheckForWeaponSwitched(int weaponNum)
        {
            if (weaponNum == newWeaponNumber)
            {
                _newWeaponTaken = true;
                _inputListener.OnSwitchWeaponPressed -= CheckForWeaponSwitched;
                OnWeaponSwitched?.Invoke();
            }
        }
    }
}