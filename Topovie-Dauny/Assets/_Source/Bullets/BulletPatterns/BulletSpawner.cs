using System;
using System.Collections.Generic;
using System.Linq;
using Bullets.BulletPools;
using Core.PoolingSystem;
using UnityEngine;

namespace Bullets.BulletPatterns
{
    public class BulletSpawner : MonoBehaviour, IPoolUser
{
    [SerializeField] private BulletSpawnerConfig config;

    private EnemyBulletPool _enemyBulletPool;
    private float _timer;
    private float _angleOffset;

    public void InjectPool(EnemyBulletPool pool)
    {
        _enemyBulletPool = pool;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (config.SpawnerType == SpawnerType.Spin)
        {
            SetSpinAngle();
        }
        else if (config.SpawnerType == SpawnerType.RadialSpin)
        {
            SetRadialSpinAngle();
        }

        if (_timer >= config.FiringRate)
        {
            Fire();
            _timer = 0;
        }
    }

    private void Fire()
    {
        if (config.SpawnerType == SpawnerType.RadialSpin)
        {
            FireRadialSpin();
        }
        else
        {
            if (_enemyBulletPool.TryGetFromPool(out var bullet))
            {
                bullet.transform.position = transform.position;
                bullet.ChangeAttributes(config.Lifetime, config.Speed, config.Damage);
                bullet.transform.rotation = transform.rotation;
            }
        }
    }

    private void SetSpinAngle()
    {
        transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + config.SpinSpeed * Time.deltaTime);
    }

    private void SetRadialSpinAngle()
    {
        _angleOffset += config.SpinSpeed * Time.deltaTime;
    }

    private void FireRadialSpin()
    {
        var bulletCount = config.BulletsPerWave;
        var angleStep = 360f / bulletCount;
        var currentAngle = _angleOffset;

        for (var i = 0; i < bulletCount; i++)
        {
            if (_enemyBulletPool.TryGetFromPool(out var bullet))
            {
                var orbitingBullet = (OrbitingEnemyBullet)bullet;
                orbitingBullet.InitOrbitingBullet(transform.position, config.SpinSpeed);
                var direction = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0);
                bullet.transform.position = transform.position + direction * config.SpawnRadius;

                bullet.ChangeAttributes(config.Lifetime, config.Speed, config.Damage);

                var rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

                currentAngle += angleStep;
            }
        }
    }
}

}