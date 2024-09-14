using System;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Dialogue UI")] 
        [SerializeField] private Image dialoguePanel;
        [SerializeField] private TMP_Text dialogueText;

        private Story currentStory;
        private bool dialogueIsPlaying;

        private void Start()
        {
            dialoguePanel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!dialogueIsPlaying)
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
            currentStory = new Story(inkJSON.text);
            dialogueIsPlaying = true;
            dialoguePanel.gameObject.SetActive(true);

            ContinueStory();
        }

        private void ExitDialogueMode()
        {
            dialogueIsPlaying = false;
            dialoguePanel.gameObject.SetActive(false);
            dialogueText.text = "";
        }

        private void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                dialogueText.text = currentStory.Continue();
            }
            else
            {
                ExitDialogueMode();
            }
        }
    }
}
