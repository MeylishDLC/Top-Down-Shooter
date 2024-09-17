using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Shop;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core
{
     public class LevelSetter: MonoBehaviour
            {
                [SerializeField] private SceneContext sceneContext;
                [SerializeField] private ShopTrigger shopTrigger;
                [SerializeField] private EnemyWave[] portalCharges;

                [Header("Time Settings")] 
                [SerializeField] private int changeStateDelayMilliseconds;
                
                private States _currentState = States.Chill;
                private int _currentPortalChargeIndex;
                private void Start()
                {
                    shopTrigger.OnChargePortalPressed += StartChargingPortal;
                }

                private void Update()
                {
                    shopTrigger.gameObject.SetActive(_currentState == States.Chill);
                }

                private void StartChargingPortal()
                {
                    _currentState = States.Fight;
                    HandleSpawningEnemies(CancellationToken.None).Forget();
                }

                private async UniTask HandleSpawningEnemies(CancellationToken token)
                {
                    var enemyWave = portalCharges[_currentPortalChargeIndex]; 
                    
                    await Spawner.SpawnEnemiesDuringTimeAsync(enemyWave.SpawnPoints, enemyWave.EnemyPrefabs, sceneContext, 
                        enemyWave.WaveDurationSeconds, enemyWave.MaxTimeBetweenSpawnMilliseconds, 
                        enemyWave.MinTimeBetweenSpawnMilliseconds, enemyWave.MaxEnemySpawnAtOnce, true, token);

                    await UniTask.Delay(changeStateDelayMilliseconds, cancellationToken: token);
                    _currentState = States.Chill;
                }
                private enum States
                {
                    Fight,
                    Chill
                }
            }
}