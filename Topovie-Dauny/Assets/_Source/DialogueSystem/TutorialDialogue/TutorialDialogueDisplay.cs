using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DialogueSystem.TutorialDialogue
{
    public class TutorialDialogueDisplay: MonoBehaviour
    {
        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;
        
        [Header("Dialogue UI")] 
        [SerializeField] private Image dialoguePanel;
        [SerializeField] private TMP_Text dialogueText;

        private InputListener _inputListener;
        private bool _isDialoguePlaying;
        private Story _currentStory;
        
        [Inject]
        public void Construct(InputListener inputListener)
        {
            _inputListener = inputListener;
            _inputListener.OnInteractPressed += HandleInput;
        }
        private void Start()
        {
            dialoguePanel.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            _inputListener.OnInteractPressed -= HandleInput;
        }
        public void EnterDialogueMode(TextAsset inkJson)
        { 
            OnDialogueStart?.Invoke();
            _inputListener.SetInput(false);
            
            _currentStory = new Story(inkJson.text);
            _isDialoguePlaying = true;
            dialoguePanel.gameObject.SetActive(true);
            
            ContinueStory();
        }
        private void HandleInput()
        {
            if (!_isDialoguePlaying)
            {
                return;
            }
            
            if (_currentStory.canContinue)
            {
                ContinueStory();
            }
            //todo: fix
            if (!_currentStory.canContinue)
            {
                ExitDialogueMode();
            }
        }
        private void ContinueStory()
        {
            if (_currentStory.canContinue)
            {
                dialogueText.text = _currentStory.Continue();
            }
            else
            {
                ExitDialogueMode();
            }
        }
        private void ExitDialogueMode()
        {
            OnDialogueEnd?.Invoke();
            _inputListener.SetInput(true);
            
            _isDialoguePlaying = false;
            dialoguePanel.gameObject.SetActive(false);
            dialogueText.text = "";
        }
    }
}