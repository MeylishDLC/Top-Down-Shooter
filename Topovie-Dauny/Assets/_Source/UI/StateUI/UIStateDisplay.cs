using System;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.StateUI
{
    public class UIStateDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text stateText;

        [Header("Text Settings")] 
        [SerializeField] private string chillStateText;
        [SerializeField] private Color chillStateTextColor;
        
        [SerializeField] private string attackStateText;
        [SerializeField] private Color attackStateTextColor; 
        
        [SerializeField] private string portalChargedStateText;
        [SerializeField] private Color portalChargedTextColor;

        private StatesChanger _statesChanger;
        
        [Inject]
        public void Construct(StatesChanger statesChanger)
        {
            _statesChanger = statesChanger;
        }
        private void Awake()
        {
            _statesChanger.OnStateChanged += ChangeStateChangerText;
        }
        private void Start()
        {
            ChangeStateChangerText(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged += ChangeStateChangerText;
        }
       
        private void ChangeStateChangerText(GameStates gameState)
        {
            switch (gameState)
            {
                case GameStates.Chill:
                    stateText.text = chillStateText;
                    stateText.color = chillStateTextColor;
                    break;
                case GameStates.PortalCharged:
                    stateText.text = portalChargedStateText;
                    stateText.color = portalChargedTextColor;
                    break;
                case GameStates.Fight:
                    stateText.text = attackStateText;
                    stateText.color = attackStateTextColor;
                    break;
                default:
                    throw new Exception("Game state not supported");
            }
        }

    }
}