using Core.InputSystem;
using Core.LevelSettings;
using DialogueSystem;
using GameEnvironment.ShopLogic.UIShop;
using UnityEngine;
using Zenject;

namespace GameEnvironment.ShopLogic
{
    public class ShopTrigger: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualQue;

        protected bool PlayerInRange;
        protected Shop Shop;
        protected DialogueManager DialogueManager;
        
        private bool _isHoldingButton;
        private float _holdStartTime;

        private StatesChanger _statesChanger;
        private InputListener _inputListener;
        
        [Inject]
        public void Construct(StatesChanger statesChanger, InputListener inputListener, Shop shop, 
            DialogueManager dialogueManager)
        {
            _statesChanger = statesChanger;
            DialogueManager = dialogueManager;
            Shop = shop;
            _inputListener = inputListener;
        }
        private void Awake()
        {
            _statesChanger.OnStateChanged += EnableOnChangeState;
            _inputListener.OnInteractPressed += ShowShop;
            
            visualQue.gameObject.SetActive(false);
            EnableOnChangeState(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= EnableOnChangeState;
            _inputListener.OnInteractPressed -= ShowShop;
        }
        private void ShowShop()
        {
            if (DialogueManager.DialogueIsPlaying)
            {
                return;
            }
            
            if (PlayerInRange)
            {
                Shop.OpenShop();
            }
        }
        private void Update()
        {
            visualQue.gameObject.SetActive(PlayerInRange);
        }
        private void EnableOnChangeState(GameStates gameState)
        {
            if (gameState != GameStates.Fight)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerInRange = false;
            }
        }
    }
}