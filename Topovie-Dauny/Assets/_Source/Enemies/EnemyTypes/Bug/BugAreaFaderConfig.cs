using UnityEngine;

namespace Enemies.EnemyTypes.Bug
{
    [CreateAssetMenu(fileName = "Bug Area Fader Config", menuName = "Combat/Enemies/Bug Area Fader Config")]
    public class BugAreaFaderConfig: ScriptableObject
    {
        [field:Header("Colors")]
        [field: SerializeField] public Color BaseColor {get; private set;}
        [field: SerializeField] public Color AttackColor {get; private set;}
        
        [field:Header("Transitions")]
        [field: SerializeField] public float ColorTransitionTime {get; private set;}
        [field: SerializeField] public float AreaFadeTime {get; private set;}
        
        [field:Header("Alpha")]
        [field:SerializeField] public float BaseAreaAlpha {get; private set;}
    }
}