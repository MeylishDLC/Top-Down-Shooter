using System;
using System.Collections.Generic;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Ink.Runtime;
using ModestTree;
using SoundSystem;
using SoundSystem.DialogueSoundSO;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace DialogueSystem
{
    public class DialogueManager
    {
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public bool DialogueIsPlaying { get; private set; }
        public bool CanEnterDialogueMode { get; private set; } = true;
        
        private AudioManager _audioManager;
        private readonly DialogueDisplay _dialogueDisplay;
        private DialogueAudioInfoSO _currentAudioInfo;
        private Dictionary<string, DialogueAudioInfoSO> _audioInfoDictionary;
        private Story _currentStory;
        private bool _hasChosen;
        private bool _canContinueLine;
        
        private const string SpeakerTag = "speaker";
        private const string SpriteTag = "sprite";
        private const string LayoutTag = "layout";
        private readonly InputListener _inputListener;
        private CancellationTokenSource _skipLineCts = new();
        
        public DialogueManager(InputListener inputListener, DialogueDisplay dialogueDisplay, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _inputListener = inputListener;
            _dialogueDisplay = dialogueDisplay;
            _currentAudioInfo = _dialogueDisplay.DefaultAudioInfo;
            
            _dialogueDisplay.gameObject.SetActive(false);
            _inputListener.OnInteractPressed += HandleInput;
            SubscribeChoices();
            InitializeAudioInfoDictionary();
        }
        public void CleanUp()
        {
            _skipLineCts?.Cancel();
            _skipLineCts?.Dispose();
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
            _dialogueDisplay.gameObject.SetActive(true);

            ResetVisuals();

            ContinueStory();
        }
        private void HandleInput()
        {
            if (!DialogueIsPlaying)
            {
                return;
            }
            
            if (_canContinueLine && _currentStory.currentChoices.IsEmpty())
            {
                ContinueStory();
            }
            else if (!_canContinueLine && _currentStory.currentChoices.IsEmpty())
            {
                CancelRecreateCts();
            }
        }
        private void MakeChoice(int choiceIndex)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
     
        private void ContinueStory()
        {
            if (_currentStory.canContinue)
            {
                var nextLine = _currentStory.Continue();
                HandleTags(_currentStory.currentTags);
                DisplayLine(nextLine);
            }
            else
            {
                ExitDialogueModeAsync(CancellationToken.None).Forget();
            }
            HandleTags(_currentStory.currentTags);
        }

        private void DisplayLine(string line)
        {
            _dialogueDisplay.DialogueText.text = line;
            _dialogueDisplay.DialogueText.maxVisibleCharacters = 0;
            HideChoices();
            
            TypeLineAsync(line, _skipLineCts.Token).Forget();
        }
        private async UniTask TypeLineAsync(string line, CancellationToken token)
        {
            try
            {
                _canContinueLine = false;
                var addingTextTagChanges = false;

                foreach (var letter in line.ToCharArray())
                {
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
                        _dialogueDisplay.DialogueText.maxVisibleCharacters++;
                        PlayDialogueSound(_dialogueDisplay.DialogueText.maxVisibleCharacters);
                        await UniTask.Delay(TimeSpan.FromSeconds(_dialogueDisplay.DialogueTypeSpeed),
                            cancellationToken: token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _dialogueDisplay.DialogueText.maxVisibleCharacters = line.Length;
            }
            finally
            {
                DisplayChoices();
                _canContinueLine = true;
            }
        }
        private async UniTask ExitDialogueModeAsync(CancellationToken token)
        {
            EnableInput();
            
            DialogueIsPlaying = false;
            _dialogueDisplay.DialoguePanel.gameObject.SetActive(false);
            _dialogueDisplay.DialogueText.text = "";

            await UniTask.Delay(TimeSpan.FromSeconds(_dialogueDisplay.DialogueRestartDelay), cancellationToken: token);
            CanEnterDialogueMode = true;
            OnDialogueEnded?.Invoke();
        }
        private void ResetVisuals()
        {
            _dialogueDisplay.NameText.text = "???";
            _dialogueDisplay.SpriteAnimator.Play("default");
            _dialogueDisplay.LayoutAnimator.Play("left");
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
                        _dialogueDisplay.NameText.text = tagValue;
                        SetCurrentAudioInfo(tagValue);
                        break;
                    case SpriteTag:
                        _dialogueDisplay.SpriteAnimator.Play(tagValue);
                        break;
                    case LayoutTag:
                        _dialogueDisplay.LayoutAnimator.Play(tagValue);
                        break;
                    default:
                        throw new Exception($"Tag isn't being handled: {tag}");
                }
            }
        }
        private void DisplayChoices()
        {
            var currentChoices = _currentStory.currentChoices;
            if (currentChoices.Count > _dialogueDisplay.Choices.Length)
            {
                throw new Exception("More choices were given than UI can support");
            }

            var index = 0;
            foreach (var choice in currentChoices)
            {
                _dialogueDisplay.Choices[index].gameObject.SetActive(true);
                _dialogueDisplay.ChoicesText[index].text = choice.text;
                index++;
            }

            for (var i = index; i < _dialogueDisplay.Choices.Length; i++)
            {
                _dialogueDisplay.Choices[i].gameObject.SetActive(false);
            }
            
            SelectFirstChoice();
        }
        private void HideChoices()
        {
            foreach (var choiceButton in _dialogueDisplay.Choices)
            {
                choiceButton.gameObject.SetActive(false);
            }
        }
        private void PlayDialogueSound(int currenDisplayedCharCount)
        {
            var typeSounds = _currentAudioInfo.TypeSounds;
            var frequencyLvl = _currentAudioInfo.FrequencyLvl;
            //setting sound effect per particular number of a char
            if (currenDisplayedCharCount % frequencyLvl == 0)
            {
                var randomIndex = Random.Range(0, typeSounds.Length);
                var soundClip = typeSounds[randomIndex];
                _audioManager.PlayOneShot(soundClip);
            }
        }
        private void InitializeAudioInfoDictionary()
        {
            _audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>
            {
                { _dialogueDisplay.DefaultAudioInfo.Id, _dialogueDisplay.DefaultAudioInfo }
            };
            
            foreach (var audioInfo in _dialogueDisplay.AudioInfos)
            {
                _audioInfoDictionary.Add(audioInfo.Id, audioInfo);
            }
        }
        private void SetCurrentAudioInfo(string id)
        {
            _audioInfoDictionary.TryGetValue(id, out var audioInfo);
            if (audioInfo is not null)
            {
                _currentAudioInfo = audioInfo;
            }
            else
            {
                Debug.LogWarning("Failed to find audio info for this id: " + id);
            }
        }
        private void CancelRecreateCts()
        {
            _skipLineCts?.Cancel();
            _skipLineCts?.Dispose();
            _skipLineCts = new CancellationTokenSource();
        }
        private void SelectFirstChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_dialogueDisplay.Choices[0].gameObject);
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
            for (var i = 0; i < _dialogueDisplay.Choices.Length; i++)
            {
                var buttonIndex = i;
                _dialogueDisplay.Choices[i].onClick.AddListener(() => MakeChoice(buttonIndex));
            }
        }
        private void UnsubscribeChoices()
        {
            for (var i = 0; i < _dialogueDisplay.Choices.Length; i++)
            {
                var buttonIndex = i;
                _dialogueDisplay.Choices[i].onClick.AddListener(() => MakeChoice(buttonIndex));
            }
        }
    }
}
