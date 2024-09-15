using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public bool DialogueIsPlaying { get; private set; }
        public bool CanEnterDialogueMode { get; private set; } = true;
        
        [Header("Dialogue UI")] 
        [SerializeField] private Image dialoguePanel;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private Animator layoutAnimator;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices; 
        private TMP_Text[] _choicesText;

        [Header("Other Settings")] 
        [SerializeField] private int dialogueRestartDelayMilliseconds;
        
        private Story _currentStory;
        private bool _hasChosen;

        private const string SPEAKER_TAG = "speaker";
        private const string SPRITE_TAG = "sprite";
        private const string LAYOUT_TAG = "layout";
        private void Start()
        {
            dialoguePanel.gameObject.SetActive(false);
            InitializeChoicesText();
        }

        private void Update()
        {
            if (!DialogueIsPlaying)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ContinueStory();
            }
        }

        public void EnterDialogueMode(TextAsset inkJSON)
        {
            _currentStory = new Story(inkJSON.text);
            DialogueIsPlaying = true;
            CanEnterDialogueMode = false;
            dialoguePanel.gameObject.SetActive(true);

            ResetVisuals();

            ContinueStory();
        }

        public void MakeChoice(int choiceIndex)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
        
        private async UniTask ExitDialogueModeAsync(CancellationToken token)
        {
            DialogueIsPlaying = false;
            dialoguePanel.gameObject.SetActive(false);
            dialogueText.text = "";

            await UniTask.Delay(dialogueRestartDelayMilliseconds, cancellationToken: token);
            CanEnterDialogueMode = true;
            
        }

        private void ResetVisuals()
        {
            nameText.text = "???";
            spriteAnimator.Play("default");
            layoutAnimator.Play("left");
        }

        private void ContinueStory()
        {
            if (_currentStory.canContinue)
            {
                dialogueText.text = _currentStory.Continue();
                DisplayChoices();
                HandleTags(_currentStory.currentTags);
            }
            else
            {
                ExitDialogueModeAsync(CancellationToken.None).Forget();
            }
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
                    case SPEAKER_TAG:
                        nameText.text = tagValue;
                        break;
                    case SPRITE_TAG:
                        spriteAnimator.Play(tagValue);
                        break;
                    case LAYOUT_TAG:
                        layoutAnimator.Play(tagValue);
                        break;
                    default:
                        throw new Exception($"Tag isn't being handled: {tag}");
                }
            }
        }
        private void DisplayChoices()
        {
            var currentChoices = _currentStory.currentChoices;
            if (currentChoices.Count > choices.Length)
            {
                throw new Exception("More choices were given than UI can support");
            }

            var index = 0;
            foreach (var choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                _choicesText[index].text = choice.text;
                index++;
            }

            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }
            
            SelectFirstChoice();
        }
        private void InitializeChoicesText()
        {
            _choicesText = new TMP_Text[choices.Length];
            
            for (int i = 0; i < choices.Length; i++)
            {
                _choicesText[i] = choices[i].GetComponentInChildren<TMP_Text>();
            }
        }
        private void SelectFirstChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        }
    }
}
