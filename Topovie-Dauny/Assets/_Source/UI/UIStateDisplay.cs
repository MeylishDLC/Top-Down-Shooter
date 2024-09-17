using System;
using Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIStateDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text stateText;

        [Header("Text Settings")] 
        [SerializeField] private string chillStateText;
        [SerializeField] private Color chillStateTextColor;
        
        [SerializeField] private string attackStateText;
        [SerializeField] private Color attackStateTextColor;

        private LevelSetter _levelSetter;
        
        [Inject]
        public void Construct(LevelSetter levelSetter)
        {
            _levelSetter = levelSetter;
        }
        private void Start()
        {
            _levelSetter.OnStateChanged += ChangeStateText;
            ChangeStateText(States.Chill);
        }
        private void ChangeStateText(States state)
        {
            if (state == States.Chill)
            {
                stateText.text = chillStateText;
                stateText.color = chillStateTextColor;
            }
            else
            {
                stateText.text = attackStateText;
                stateText.color = attackStateTextColor;
            }
        }
    }
}