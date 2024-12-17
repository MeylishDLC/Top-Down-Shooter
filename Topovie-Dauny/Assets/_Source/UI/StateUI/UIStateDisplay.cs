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
        [SerializeField] private Image attackPanel;
        [SerializeField] private Image portalOpenedPanel;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float slideInValue;
        [SerializeField] private float slideOutValue;
        
        private StatesChanger _statesChanger;
        private CancellationToken _destroyCancellationToken;
        
        [Inject]
        public void Construct(StatesChanger statesChanger)
        {
            _statesChanger = statesChanger;
        }
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _statesChanger.OnStateChanged += ChangeStateChangerText;
        }
        private void Start()
        {
            attackPanel.gameObject.SetActive(false);
            portalOpenedPanel.gameObject.SetActive(false);
            HidePanel(portalOpenedPanel, _destroyCancellationToken).Forget();
            ChangeStateChangerText(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= ChangeStateChangerText;
        }
        private void ChangeStateChangerText(GameStates gameState)
        {
            switch (gameState)
            {
                case GameStates.Chill:
                    HidePanel(attackPanel, _destroyCancellationToken).Forget();
                    break;
                case GameStates.PortalCharged:
                    HidePanel(attackPanel, _destroyCancellationToken).
                        ContinueWith(() => ShowPanel(portalOpenedPanel, _destroyCancellationToken)).Forget();
                    break;
                case GameStates.Fight:
                    ShowPanel(attackPanel, _destroyCancellationToken).Forget();
                    break;
                default:
                    throw new Exception("Game state not supported");
            }
        }
        private UniTask ShowPanel(Image panel, CancellationToken token)
        {
            panel.gameObject.SetActive(true);
            return panel.gameObject.transform.DOLocalMoveY(slideInValue, animationDuration).ToUniTask(cancellationToken: token);
        }
        private UniTask HidePanel(Image panel, CancellationToken token)
        {
            return panel.gameObject.transform.DOLocalMoveY(slideOutValue, animationDuration).ToUniTask(cancellationToken: token)
                .ContinueWith(() => panel.gameObject.SetActive(false));
        }
    }
}