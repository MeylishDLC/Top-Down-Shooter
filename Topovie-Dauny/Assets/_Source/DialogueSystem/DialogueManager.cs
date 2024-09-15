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

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices; 
        private TMP_Text[] _choicesText;

        [Header("Other Settings")] 
        [SerializeField] private int dialogueRestartDelayMilliseconds;
        
        private Story _currentStory;
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

        private void ContinueStory()
        {
            if (_currentStory.canContinue)
            {
                dialogueText.text = _currentStory.Continue();
                DisplayChoices();
            }
            else
            {
                ExitDialogueModeAsync(CancellationToken.None).Forget();
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
