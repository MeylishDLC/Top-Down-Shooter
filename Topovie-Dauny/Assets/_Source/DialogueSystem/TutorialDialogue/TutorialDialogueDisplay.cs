using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Ink.Runtime;
using ModestTree;
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

        [Header("Time Settings")] 
        [SerializeField] private float dialogueTypeSpeed;

        private InputListener _inputListener;
        private bool _isDialoguePlaying;
        private bool _canContinueLine;
        private Story _currentStory;
        
        //todo disable player rotating script
        
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
            _inputListener.SetInput(false, true);
            
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
            
            if (_canContinueLine && _currentStory.currentChoices.IsEmpty())
            {
                ContinueStory();
            }
        }
        private void ContinueStory()
        {
            if (_currentStory.canContinue)
            {
                var nextLine = dialogueText.text = _currentStory.Continue();
                DisplayLineAsync(nextLine, CancellationToken.None).Forget();
            }
            else
            {
                ExitDialogueMode();
            }
        }
        private async UniTask DisplayLineAsync(string line, CancellationToken token)
        {
            dialogueText.text = line;
            dialogueText.maxVisibleCharacters = 0;

            _canContinueLine = false;

            var addingTextTagChanges = false;
            try
            {
                foreach (var letter in line.ToCharArray())
                {
                    if (token.IsCancellationRequested)
                    {
                        dialogueText.maxVisibleCharacters = line.Length;
                        break;
                    }
                    //check for ink tags
                    if (letter == '<' || addingTextTagChanges)
                    {
                        addingTextTagChanges = true;
                        if (letter == '>')
                        {
                            addingTextTagChanges = false;
                        }
                    }
                    else
                    {
                        dialogueText.maxVisibleCharacters++;
                        await UniTask.Delay(TimeSpan.FromSeconds(dialogueTypeSpeed),
                            cancellationToken: token);
                    }
                }
            }
            catch
            {
                //
            }
            finally
            {
                _canContinueLine = true;
            }
        }
        private void ExitDialogueMode()
        {
            OnDialogueEnd?.Invoke();
            _inputListener.SetInput(true, true);
            
            _isDialoguePlaying = false;
            dialoguePanel.gameObject.SetActive(false);
            dialogueText.text = "";
        }
    }
}