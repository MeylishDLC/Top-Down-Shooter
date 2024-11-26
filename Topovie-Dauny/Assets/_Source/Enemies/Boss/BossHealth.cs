using UnityEngine;

namespace Enemies.Boss
{
    public class BossHealth: MonoBehaviour, IEnemyHealth
    {
        public void TakeDamage(int damage)
        {
            Debug.Log("Boss Health Damage");
        }
    }
}