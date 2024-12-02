using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Core.InputSystem;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InteractableNPC
{
    public class Npc: MonoBehaviour
    {
        [SerializeField] private float npcFadeInDuration;
        [SerializeField] private float npcFadeOutDuration;
        [SerializeField] private float npcStayAfterDialogueDuration;
        [SerializeField] private NpcDialogueBubble npcDialogueBubble;
        [SerializeField] private SpriteRenderer visualQue;
        
        private SpriteRenderer _npcRenderer;
        private CancellationToken _destroyCancellationToken;
        private bool _wasTalkedTo;
        private bool _isPlayerInRange;
        
        private StatesChanger _statesChanger;
        private InputListener _inputListener;
        private NpcVisual _npcVisual;
        
        [Inject]
        public void Construct(StatesChanger statesChanger, InputListener inputListener)
        {
            _statesChanger = statesChanger;
            _inputListener = inputListener;
        }
        private void Awake()
        {
            _statesChanger.OnStateChanged += SetVisibleOnStateChange;
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _npcRenderer = GetComponent<SpriteRenderer>();
            _npcVisual = new NpcVisual(_npcRenderer, npcDialogueBubble, npcFadeInDuration, npcFadeOutDuration, npcStayAfterDialogueDuration);
            SetVisualQue(false);
            
            _npcVisual.OnDisappearCompletely += DestroyOnInteractEnd;
            _inputListener.OnInteractPressed += OnInteract;
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= SetVisibleOnStateChange;
            _inputListener.OnInteractPressed -= OnInteract;
            _npcVisual.OnDisappearCompletely -= DestroyOnInteractEnd;
            _npcVisual.CleanUp();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = true;
                if (!_wasTalkedTo)
                {
                    SetVisualQue(true);
                }
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = false;
                SetVisualQue(false);
            }
        }
        private void OnInteract()
        {
            if (!_isPlayerInRange || !gameObject.activeSelf || _wasTalkedTo)
            {
                return;
            }
            
            SetVisualQue(false);
            _wasTalkedTo = true;
            npcDialogueBubble.DisplayDialogueAsync(_destroyCancellationToken).Forget();
        }
        private void SetVisibleOnStateChange(GameStates state)
        {
            if (_wasTalkedTo)
            {
                return;
            }
            
            if (state != GameStates.Fight)
            {
                _npcVisual.Appear(_destroyCancellationToken).Forget();
            }
            else
            {
                _npcVisual.Disappear(_destroyCancellationToken).Forget();
            }
        }
        private void SetVisualQue(bool enable)
        {
            visualQue.gameObject.SetActive(enable);
        }
        private void DestroyOnInteractEnd() => Destroy(gameObject);
    }
}