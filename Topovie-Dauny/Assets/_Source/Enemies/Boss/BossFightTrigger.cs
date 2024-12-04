using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cinemachine;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using UnityEngine;
using Zenject;

namespace Enemies.Boss
{
    [RequireComponent(typeof(Collider2D))]
    public class BossFightTrigger: MonoBehaviour
    {
        public event Action OnBossFightStarted;
        
        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera leoCamera;
        [SerializeField] private float delayBeforeDialogue;
        [SerializeField] private float delayBeforeStartFight;
        [SerializeField] private TextAsset dialogueBeforeFight;
        [SerializeField] private SpriteRenderer wall;
        [SerializeField] private float wallFadeInDuration;

        private Collider2D _collider;
        private DialogueManager _dialogueManager;
        private InputListener _inputListener;
        private int _mainCamPriority;
        private CancellationToken _destroyCancellationToken;

        [Inject]
        public void Construct(DialogueManager dialogueManager, InputListener inputListener)
        {
            _inputListener = inputListener;
            _dialogueManager = dialogueManager;
        }
        private void Awake()
        {
            wall.gameObject.SetActive(false);
            wall.DOFade(0f, 0f);
            
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _collider = GetComponent<Collider2D>();
            _mainCamPriority = mainCamera.Priority;
            leoCamera.Priority = _mainCamPriority-2; 
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _collider.enabled = false;
                PlayCutsceneAsync(_destroyCancellationToken).Forget();
            }
        }
        private async UniTask PlayCutsceneAsync(CancellationToken token)
        {
            _inputListener.SetInput(false, true);
            leoCamera.Priority = _mainCamPriority++;
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeDialogue), cancellationToken: token);
            PlayDialogue();
        }
        private void PlayDialogue()
        {
            _dialogueManager.OnDialogueEnded += SwitchToMainCamera;
            _dialogueManager.EnterDialogueMode(dialogueBeforeFight);
        }
        private void SwitchToMainCamera()
        {
            _dialogueManager.OnDialogueEnded -= SwitchToMainCamera; 
            SwitchToMainCameraAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask SwitchToMainCameraAsync(CancellationToken token)
        {
            leoCamera.Priority = _mainCamPriority-2; 
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeStartFight), cancellationToken: token);
            await EnableWall(token);
            _inputListener.SetInput(true);
            OnBossFightStarted?.Invoke();
        }

        private async UniTask EnableWall(CancellationToken token)
        {
            wall.gameObject.SetActive(true);
            await wall.DOFade(1f, wallFadeInDuration).ToUniTask(cancellationToken: token);
        }
    }
}