using System;
using System.Linq;
using System.Threading;
using Cinemachine;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DialogueSystem;
using Enemies.Boss.Phases;
using FMOD.Studio;
using SoundSystem;
using UI.Menus;
using UnityEngine;
using Zenject;

namespace Enemies.Boss
{
    public class BossLeo : MonoBehaviour
    {
        public event Action OnBossDefeated;
        
        [SerializeField] private GameOverScreen gameOverScreen;
        [SerializeField] private BossFightTrigger bossFightTrigger;
        [SerializeField] private SerializedDictionary<BossPhase, TextAsset> phaseDialoguePair;
        [SerializeField] private BossHealth bossHealth;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer headRenderer;
        [SerializeField] private Sprite attackSprite;
        [SerializeField] private Sprite vulnerableSprite;
        [SerializeField] private float hurtDuration;
        
        [Header("Cam Settings")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float newZoomValue = 1.6f;
        [SerializeField] private float camZoomDuration;
        
        private int _currentPhaseIndex;
        private CancellationToken _destroyCancellationToken;
        private StatesChanger _statesChanger;
        private DialogueManager _dialogueManager;
        private BossLeoVisual _bossLeoVisual;
        private AudioManager _audioManager;
        
        [Inject]
        public void Construct(StatesChanger statesChanger, DialogueManager dialogueManager, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _statesChanger = statesChanger;
            _dialogueManager = dialogueManager;
        }
        private void Start()
        {
            _bossLeoVisual = new BossLeoVisual(headRenderer, attackSprite, vulnerableSprite, hurtDuration, _destroyCancellationToken);
            bossHealth.OnDamageTaken += _bossLeoVisual.ShowLeoHurt;
            
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            bossHealth.OnPhaseFinished += EndPhase;
            bossFightTrigger.OnBossFightStarted += StartFight;
            gameOverScreen.OnScreenFaded += DestroyOnGameOver;
        }
        private void OnDestroy()
        {
            bossHealth.OnPhaseFinished -= EndPhase;
            bossHealth.OnDamageTaken -= _bossLeoVisual.ShowLeoHurt;
            gameOverScreen.OnScreenFaded -= DestroyOnGameOver;
        }
        private void StartFight()
        {
            bossFightTrigger.OnBossFightStarted -= StartFight;
            ChangeCamZoom(_destroyCancellationToken).Forget();
            _audioManager.ChangeMusic(_audioManager.FMODEvents.BossFightMusic, STOP_MODE.ALLOWFADEOUT);
            _statesChanger.ChangeState(GameStates.Fight);
            StartPhase();
        }
        private UniTask ChangeCamZoom(CancellationToken token)
        {
            return DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
                x => virtualCamera.m_Lens.OrthographicSize = x, newZoomValue, 0.3f)
                .ToUniTask(cancellationToken: token);
        }
        private void EndPhase()
        {
            phaseDialoguePair.Keys.ElementAt(_currentPhaseIndex).FinishPhase();
            PlayDialogue();
        }
        private void StartPhase()
        {
            _dialogueManager.OnDialogueEnded -= StartPhase;
            
            StartPhaseAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask StartPhaseAsync(CancellationToken token)
        {
            if (_currentPhaseIndex >= phaseDialoguePair.Count)
            {
                _statesChanger.ChangeState(GameStates.PortalCharged);
                _bossLeoVisual.SetLeoHurt();
                OnBossDefeated?.Invoke();
                return;
            }

            var phase = phaseDialoguePair.Keys.ElementAt(_currentPhaseIndex);
           
            if (_currentPhaseIndex > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(phase.PhaseConfig.AttackTransitionDuration),
                    cancellationToken: token);
            }
            bossHealth.ChangePhase(phase);
            phase.StartPhase();
        }
        private void PlayDialogue()
        {
            _dialogueManager.OnDialogueEnded += StartPhase;
            var dialogue = phaseDialoguePair.Values.ElementAt(_currentPhaseIndex);
            _currentPhaseIndex++;
            if (dialogue is null)
            {
                Debug.LogWarning($"No dialogue for {_currentPhaseIndex} phase}}");
                StartPhase();
                return;
            }
            _dialogueManager.EnterDialogueMode(dialogue);
        }
        private void DestroyOnGameOver() => Destroy(gameObject);
    }
}
