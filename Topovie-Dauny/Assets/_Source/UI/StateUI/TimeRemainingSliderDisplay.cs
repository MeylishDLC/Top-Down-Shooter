using System;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.StateUI
{
    public class TimeRemainingSliderDisplay: MonoBehaviour
    {
        [SerializeField] private Slider timeRemainingSlider;
        [SerializeField] private float sliderMoveYOnDisappear = 1f;
        [SerializeField] private float sliderMoveYOnAppear = -60f;
        [SerializeField] private float sliderDisappearDuration = 0.5f;
        
        private LevelChargesHandler _levelChargesHandler;
        private StatesChanger _statesChanger;

        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler, StatesChanger statesChanger)
        {
            _levelChargesHandler = levelChargesHandler;
            _statesChanger = statesChanger;
        }
        private void Awake()
        {
            _levelChargesHandler.OnTimeRemainingChanged += UpdateSliderValue;
            _statesChanger.OnStateChanged += EnableSliderOnChangeStateChanger;
        }
        private void Start()
        {
            timeRemainingSlider.transform.DOMoveY(sliderMoveYOnDisappear, 0f);
            timeRemainingSlider.value = 1f;
            timeRemainingSlider.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            _levelChargesHandler.OnTimeRemainingChanged -= UpdateSliderValue;
            _statesChanger.OnStateChanged -= EnableSliderOnChangeStateChanger;
        }
        private void UpdateSliderValue(float remainingTime, float duration)
        {
            if (timeRemainingSlider != null)
            {
                timeRemainingSlider.value = remainingTime / duration;
            }
        }
        private void EnableSliderOnChangeStateChanger(GameStates gameState)
        {
            if(gameState == GameStates.Fight)
            {
                if (!timeRemainingSlider.gameObject.activeSelf)
                {
                    SliderAppearAsync(CancellationToken.None).Forget();
                }
            }
            else
            {
                if (timeRemainingSlider.gameObject.activeSelf)
                {
                    SliderDisappearAsync(CancellationToken.None).Forget();
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