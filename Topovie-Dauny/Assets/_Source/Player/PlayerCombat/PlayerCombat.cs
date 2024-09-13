using System;
using UnityEngine;

namespace Player.PlayerCombat
{
    public class PlayerCombat: MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Animator firingPointAnimator;
        [Range(0.1f, 1f)] [SerializeField] private float fireRate = 0.5f;

        private float fireTimer;
        private static readonly int shoot = Animator.StringToHash("shoot");
        private void Update()
        {
            if (Input.GetMouseButton(0) && fireTimer <= 0)
            {
                Shoot();
                fireTimer = fireRate;
            }
            else
            {
                fireTimer -= Time.deltaTime;
            }
        }

        private void Shoot()
        {
            firingPointAnimator.SetTrigger(shoot);
            var firingPointTransform = firingPointAnimator.transform;
            Instantiate(bulletPrefab, firingPointTransform.position, firingPointTransform.rotation);
        }
    }
}