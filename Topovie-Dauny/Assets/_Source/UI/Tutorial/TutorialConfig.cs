using DialogueSystem;
using UnityEngine;

namespace UI.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Tutorial/TutorialConfig")]
    public class TutorialConfig : ScriptableObject
    {
        [field: Header("Timing Settings")]
        [field: SerializeField] public float TimeOnWasdChecked { get; private set; }
        [field: SerializeField] public float TimeOnShootingChecked { get; private set; }
        [field: SerializeField] public float TimeOnAbilitiesChecked { get; private set; }
        [field: SerializeField] public float TimeOnCameraSlideToShop { get; private set; }
        [field: SerializeField] public float TimeOnCameraSlideToCharge { get; private set; }
        [field: SerializeField] public float TimeOnCameraSlideToPlayer { get; private set; }
        
        [field: Header("Dialogues")]
        [field: SerializeField] public TextAsset OnTutorialStarted {get; private set;}
        [field: SerializeField] public TextAsset OnWasdExplained {get; private set;}
        [field: SerializeField] public TextAsset OnShootingExplained {get; private set;}
        [field: SerializeField] public TextAsset OnHealthExplained {get; private set;}
        [field: SerializeField] public TextAsset OnAbilitiesExplained {get; private set;}
        [field: SerializeField] public TextAsset OnShopExplained {get; private set;}
        [field: SerializeField] public TextAsset OnChargeExplained {get; private set;}
        [field: SerializeField] public TextAsset OnTutorialEnded {get; private set;}

    }
}