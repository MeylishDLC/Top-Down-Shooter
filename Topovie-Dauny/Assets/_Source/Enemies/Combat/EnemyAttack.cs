using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies.Combat
{
    public class EnemyAttack: MonoBehaviour
    {
        [field:SerializeField] public int Attack { get; private set; }
    }
}