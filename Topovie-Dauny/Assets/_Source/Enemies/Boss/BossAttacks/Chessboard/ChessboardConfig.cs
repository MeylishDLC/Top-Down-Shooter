using UnityEngine;

namespace Enemies.Boss.BossAttacks.Chessboard
{
    [CreateAssetMenu(fileName = "Chessboard Config", menuName = "Combat/BossAttacks/Chessboard Config")]
    public class ChessboardConfig: ScriptableObject
    {
        [field:Header("Colors")]
        [field:SerializeField] public Color WarningTileColor {get; private set;}
        [field:SerializeField] public Color AttackingTileColor {get; private set;} = Color.white;

        [field:Header("Warning Time Settings")] 
        [field:SerializeField] public float FadeInDuration {get; private set;}
        [field:SerializeField] public float WarningDuration {get; private set;}
        [field:SerializeField] public int WarningBlinkAmount {get; private set;}
        
        [field:Header("Attack Settings")]
        [field:SerializeField] public float AttackDuration {get; private set;}
        [field:SerializeField] public float DelayBeforeAttack {get; private set;}
        [field:SerializeField] public float FadeOutDuration {get; private set;}
    }
}