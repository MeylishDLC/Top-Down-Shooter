using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace UI
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

        private LevelSetter _levelSetter;
        
        [Inject]
        public void Construct(LevelSetter levelSetter)
        {
            _levelSetter = levelSetter;
        }
        private void Awake()
        {
            _levelSetter.OnStateChanged += ChangeStateText;
            _levelSetter.OnStateChanged += EnableSliderOnChangeState;
            Spawner.OnFightStateStartTime += StartTimeTracking;
        }
        private void Start()
        {
            ChangeStateText(States.Chill);
            timeRemainingSlider.transform.DOMoveY(sliderMoveYOnDisappear, 0f);
            timeRemainingSlider.value = 1f;
            timeRemainingSlider.gameObject.SetActive(false);
        }
        void OnDestroy()
        {
            Spawner.OnFightStateStartTime -= StartTimeTracking;
            _levelSetter.OnStateChanged -= ChangeStateText;
            _levelSetter.OnStateChanged -= EnableSliderOnChangeState;
        }
        
        private void UpdateSliderValue(float remainingTime, float duration)
        {
            if (timeRemainingSlider != null)
            {
                timeRemainingSlider.value = remainingTime / duration;
            }
        }
        private void StartTimeTracking(float startTime, float duration)
        {
            UpdateTimeRemainingAsync(startTime, duration, CancellationToken.None).Forget();
        }

        private async UniTask UpdateTimeRemainingAsync(float startTime, float duration, CancellationToken token)
        {
            timeRemainingSlider.value = 1f;

            while (Time.time - startTime < duration)
            {
                var remainingTime = duration - (Time.time - startTime);
                UpdateSliderValue(remainingTime, duration);
                await UniTask.Yield();
            }

            UpdateSliderValue(0, duration);
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

        private void EnableSliderOnChangeState(States state)
        {
            if (state == States.Chill)
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