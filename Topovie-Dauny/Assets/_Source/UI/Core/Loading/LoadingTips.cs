using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Core.Loading
{
    public class LoadingTips
    {
        private readonly LoadingTipsConfig _config;
        
        [Inject]
        public LoadingTips(LoadingTipsConfig config)
        {
            _config = config;
        }

        public void DisplayRandomTip(TMP_Text text)
        {
            var randomTip = _config.Tips[Random.Range(0, _config.Tips.Count)];
            
            text.text = randomTip;
        }
    }
}