using System;
using System.Collections.Generic;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Ink.Runtime;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace DialogueSystem
{
    public class DialogueManager
    {
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public bool DialogueIsPlaying { get; private set; }
        public bool CanEnterDialogueMode { get; private set; } = true;
        
        private DialogueDisplay _currentDialogueDisplay;
        
        private Story _currentStory;
        private bool _hasChosen;

        private const string SpeakerTag = "speaker";
        private const string SpriteTag = "sprite";
        private const string LayoutTag = "layout";
        private readonly InputListener _inputListener;

        public DialogueManager(InputListener inputListener, DialogueDisplay dialogueDisplay)
        {
            _inputListener = inputListener;
            _currentDialogueDisplay = dialogueDisplay;
            
            _currentDialogueDisplay.gameObject.SetActive(false);
            _inputListener.OnInteractPressed += HandleInput;
            SubscribeChoices();
        }
        //todo call it on scene unload
        public void Expose()
        {
            _inputListener.OnInteractPressed -= HandleInput;
            UnsubscribeChoices();
        }
        public void EnterDialogueMode(TextAsset inkJson)
        { 
            OnDialogueStarted?.Invoke();
            DisableInput();
            
            _currentStory = new Story(inkJson.text);
            DialogueIsPlaying = true;
            CanEnterDialogueMode = false;
            _currentDialogueDisplay.gameObject.SetActive(true);

            ResetVisuals();

            ContinueStory();
        }
        public void MakeChoice(int choiceIndex)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
        private void HandleInput()
        {
            if (!DialogueIsPlaying)
            {
                return;
            }
            
            if (_currentStory.canContinue && _currentStory.currentChoices.IsEmpty())
            {
                ContinueStory();
            }
            //todo: fix
            if (!_currentStory.canContinue && _currentStory.currentChoices.IsEmpty())
            {
                ExitDialogueModeAsync(CancellationToken.None).Forget();
            }
        }
        private void ContinueStory()
        {
            if (_currentStory.canContinue)
            {
                _currentDialogueDisplay.DialogueText.text = _currentStory.Continue();
                DisplayChoices();
                HandleTags(_currentStory.currentTags);
            }
            else
            {
                ExitDialogueModeAsync(CancellationToken.None).Forget();
            }
        }
        private async UniTask ExitDialogueModeAsync(CancellationToken token)
        {
            EnableInput();
            
            DialogueIsPlaying = false;
            _currentDialogueDisplay.DialoguePanel.gameObject.SetActive(false);
            _currentDialogueDisplay.DialogueText.text = "";

            await UniTask.Delay(_currentDialogueDisplay.DialogueRestartDelayMilliseconds, cancellationToken: token);
            CanEnterDialogueMode = true;
            OnDialogueEnded?.Invoke();
        }
        private void ResetVisuals()
        {
            _currentDialogueDisplay.NameText.text = "???";
            _currentDialogueDisplay.SpriteAnimator.Play("default");
            _currentDialogueDisplay.LayoutAnimator.Play("left");
        }
        private void HandleTags(List<string> currentTags)
        {
            foreach (var tag in currentTags)
            {
                var splitTag = tag.Split(':');
                if (splitTag.Length != 2)
                {
                    throw new Exception($"Tag couldn't be parsed: {tag}");
                }

                var tagKey = splitTag[0].Trim();
                var tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SpeakerTag:
                        _currentDialogueDisplay.NameText.text = tagValue;
                        break;
                    case SpriteTag:
                        _currentDialogueDisplay.SpriteAnimator.Play(tagValue);
                        break;
                    case LayoutTag:
                        _currentDialogueDisplay.LayoutAnimator.Play(tagValue);
                        break;
                    default:
                        throw new Exception($"Tag isn't being handled: {tag}");
                }
            }
        }
        private void DisplayChoices()
        {
            var currentChoices = _currentStory.currentChoices;
            if (currentChoices.Count > _currentDialogueDisplay.Choices.Length)
            {
                throw new Exception("More choices were given than UI can support");
            }

            var index = 0;
            foreach (var choice in currentChoices)
            {
                _currentDialogueDisplay.Choices[index].gameObject.SetActive(true);
                _currentDialogueDisplay.ChoicesText[index].text = choice.text;
                index++;
            }

            for (int i = index; i < _currentDialogueDisplay.Choices.Length; i++)
            {
                _currentDialogueDisplay.Choices[i].gameObject.SetActive(false);
            }
            
            SelectFirstChoice();
        }
        private void SelectFirstChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_currentDialogueDisplay.Choices[0].gameObject);
        }
        private void DisableInput()
        {
            _inputListener.SetFiringAbility(false);
            _inputListener.SetUseAbility(false);
        }
        private void EnableInput()
        {
            _inputListener.SetFiringAbility(true);
            _inputListener.SetUseAbility(true);
        }
        private void SubscribeChoices()
        {
            for (int i = 0; i < _currentDialogueDisplay.Choices.Length; i++)
            {
                var buttonIndex = i;
                _currentDialogueDisplay.Choices[i].onClick.AddListener(() => MakeChoice(buttonIndex));
            }
        }
        private void UnsubscribeChoices()
        {
            for (int i = 0; i < _currentDialogueDisplay.Choices.Length; i++)
            {
                var buttonIndex = i;
                _currentDialogueDisplay.Choices[i].onClick.AddListener(() => MakeChoice(buttonIndex));
            }
        }
    }
}
