using Bullets;
using UnityEngine;

public class BulletSpawner: MonoBehaviour
{
    private enum SpawnerType
    {
        Straight,
        Spin
    }

    [Header("Bullet Attributes")] 
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private float lifetime;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
        
    [Header("Spawn Attributes")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float firingRate;

    private EnemyBullet _spawnedBullet;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (spawnerType == SpawnerType.Spin)
        {
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f);
            if (_timer >= firingRate)
            {
                Fire();
                _timer = 0;
            }
        }
    }
    private void Fire()
    {
        if (bulletPrefab)
        {
            _spawnedBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            _spawnedBullet.ChangeAttributes(lifetime, speed, damage);
            _spawnedBullet.transform.rotation = transform.rotation;
        }
    }
}