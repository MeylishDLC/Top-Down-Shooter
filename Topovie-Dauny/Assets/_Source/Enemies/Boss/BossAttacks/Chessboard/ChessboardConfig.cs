using Enemies.Boss.BossAttacks.Lasers;
using UnityEngine;

namespace Enemies.Boss.BossAttacks.Chessboard
{
    [CreateAssetMenu(fileName = "Chessboard Config", menuName = "Combat/BossAttacks/Chessboard Config")]
    public class ChessboardConfig: BaseBossAttackConfig
    {
        [field:Header("Colors")]
        [field:SerializeField] public Color WarningTileColor {get; private set;}
        [field:SerializeField] public Color AttackingTileColor {get; private set;} = Color.white;

        [field:Header("Other Time Settings")] 
        [field:SerializeField] public int WarningBlinkAmount {get; private set;}
        [field:SerializeField] public float DelayBeforeAttack {get; private set;}
    }
}