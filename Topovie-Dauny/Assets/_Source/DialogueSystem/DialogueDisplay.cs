using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueDisplay: MonoBehaviour
    {
        [field:Header("Dialogue UI")] 
        [field: SerializeField] public Image DialoguePanel {get; private set;}
        [field: SerializeField] public TMP_Text DialogueText {get; private set;}
        [field: SerializeField] public TMP_Text NameText {get; private set;}
        [field: SerializeField] public Animator SpriteAnimator {get; private set;}
        [field: SerializeField] public Animator LayoutAnimator {get; private set;}

        [field:Header("Choices UI")]
        [field: SerializeField] public Button[] Choices {get; private set;}
        public TMP_Text[] ChoicesText {get; private set;}

        [field:Header("Time Settings")] 
        [field: SerializeField] public float DialogueRestartDelay {get; private set;}
        [field: SerializeField] public float DialogueTypeSpeed {get; private set;}

        private void Start()
        {
            InitializeChoicesText();
        }
        private void InitializeChoicesText()
        {
            ChoicesText = new TMP_Text[Choices.Length];
            
            for (int i = 0; i < Choices.Length; i++)
            {
                ChoicesText[i] = Choices[i].GetComponentInChildren<TMP_Text>();
            }
        }
    }
}