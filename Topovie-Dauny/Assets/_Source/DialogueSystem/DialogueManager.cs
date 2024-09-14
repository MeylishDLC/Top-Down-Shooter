using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ink.Runtime;
using TMPro;
using UnityEngine;
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

        [Header("Other Settings")] 
        [SerializeField] private int dialogueRestartDelayMilliseconds;
        
        private Story _currentStory;
        private void Start()
        {
            dialoguePanel.gameObject.SetActive(false);
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
            }
            else
            {
                ExitDialogueModeAsync(CancellationToken.None).Forget();
            }
        }
    }
}
