using System.Threading;
using Core.InputSystem;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Interactable.NPCstuff
{
    public class Npc: MonoBehaviour
    {
        [SerializeField] private float npcStayAfterDialogueDuration;
        [SerializeField] private NpcDialogueBubble npcDialogueBubble;
        [SerializeField] private SpriteRenderer visualQue;
        
        private CancellationToken _destroyCancellationToken;
        private bool _wasTalkedTo;
        private bool _isPlayerInRange;
        private bool _isAvailableForInteraction = true;
        
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
            _statesChanger.OnStateChanged += SetCanInteractOnChangeState;
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _npcVisual = new NpcVisual(npcDialogueBubble, npcStayAfterDialogueDuration);
            SetVisualQue(false);
            
            _inputListener.OnInteractPressed += OnInteract;
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= SetVisibleOnStateChange;
            _statesChanger.OnStateChanged -= SetCanInteractOnChangeState;
            _inputListener.OnInteractPressed -= OnInteract;
            _npcVisual.CleanUp();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isAvailableForInteraction)
            {
                return;
            }
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
            if (!_isAvailableForInteraction)
            {
                return;
            }
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
        private void SetVisualQue(bool enable)
        {
            visualQue.gameObject.SetActive(enable);
        }

        private void SetVisibleOnStateChange(GameStates state)
        {
            if (_wasTalkedTo)
            {
                return;
            }
            
            if (state != GameStates.Fight)
            {
            }
            else
            {
                
            }
        }
        private void SetCanInteractOnChangeState(GameStates state)
        {
            if (_wasTalkedTo)
            {
                return;
            }
            
            if (state != GameStates.Fight)
            {
                _isAvailableForInteraction = true;
            }
            else
            {
                _isAvailableForInteraction = false;
                SetVisualQue(false);
            }
        }
    }
}