﻿using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.PlayerGUI
{
    public class UIStateDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text stateText;
        
        [Header("Slider Settings")]
        [SerializeField] private Slider timeRemainingSlider;
        [SerializeField] private float sliderMoveYOnDisappear;
        [SerializeField] private float sliderMoveYOnAppear;
        [SerializeField] private float sliderDisappearDuration;

        [Header("Text Settings")] 
        [SerializeField] private string chillStateText;
        [SerializeField] private Color chillStateTextColor;
        
        [SerializeField] private string attackStateText;
        [SerializeField] private Color attackStateTextColor;

        private LevelChargesHandler _levelChargesHandler;
        
        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler)
        {
            _levelChargesHandler = levelChargesHandler;
        }
        private void Awake()
        {
            _levelChargesHandler.OnStateChanged += ChangeStateText;
            _levelChargesHandler.OnStateChanged += EnableSliderOnChangeState;
            _levelChargesHandler.OnTimeRemainingChanged += UpdateSliderValue;
        }
        private void Start()
        {
            ChangeStateText(GameStates.Chill);
            timeRemainingSlider.transform.DOMoveY(sliderMoveYOnDisappear, 0f);
            timeRemainingSlider.value = 1f;
            timeRemainingSlider.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            _levelChargesHandler.OnStateChanged -= ChangeStateText;
            _levelChargesHandler.OnStateChanged -= EnableSliderOnChangeState;
            _levelChargesHandler.OnTimeRemainingChanged -= UpdateSliderValue;
        }
        private void UpdateSliderValue(float remainingTime, float duration)
        {
            if (timeRemainingSlider != null)
            {
                timeRemainingSlider.value = remainingTime / duration;
            }
        }
        private void ChangeStateText(GameStates gameState)
        {
            if (gameState == GameStates.Chill)
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

        private void EnableSliderOnChangeState(GameStates gameState)
        {
            if (gameState == GameStates.Chill)
            {
                if (timeRemainingSlider.gameObject.activeSelf)
                {
                    SliderDisappearAsync(CancellationToken.None).Forget();
                }
            }
            else
            {
                if (!timeRemainingSlider.gameObject.activeSelf)
                {
                    SliderAppearAsync(CancellationToken.None).Forget();
                }
            }
        }
        
        private async UniTask SliderAppearAsync(CancellationToken token)
        {
            timeRemainingSlider.gameObject.SetActive(true);
            await timeRemainingSlider.gameObject.transform.DOLocalMoveY(sliderMoveYOnAppear, sliderDisappearDuration).ToUniTask(cancellationToken: token);
        }
        private async UniTask SliderDisappearAsync(CancellationToken token)
        {
            await timeRemainingSlider.gameObject.transform.DOLocalMoveY(sliderMoveYOnDisappear, sliderDisappearDuration).ToUniTask(cancellationToken: token);
            timeRemainingSlider.gameObject.SetActive(false);
        }

    }
}