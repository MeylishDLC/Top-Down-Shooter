using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Core.Loading
{
    public class LoadingScreenComponents
    {
        public RectTransform LoadingScreen {get; private set;}
        public Slider LoadingSlider {get; private set;}
        public TMP_Text TipText{get; private set;}

        public LoadingScreenComponents(RectTransform loadingScreen, Slider loadingSlider, TMP_Text tipText)
        {
            LoadingScreen = loadingScreen;
            LoadingSlider = loadingSlider;
            TipText = tipText;
        }
    }
}