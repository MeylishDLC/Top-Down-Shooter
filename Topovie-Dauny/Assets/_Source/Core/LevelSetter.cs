using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Shop;
using UnityEngine;
using Zenject;

namespace Core
{
     public class LevelSetter: MonoBehaviour
     {
         public event Action<States> OnStateChanged;

         [SerializeField] private SceneContext sceneContext;
         [SerializeField] private ShopTrigger shopTrigger;
         [SerializeField] private Transform[] allSpawns;
         [SerializeField] private EnemyWave[] portalCharges;

         [Header("Time Settings")] [SerializeField]
         private int changeStateDelayMilliseconds;

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
             OnStateChanged?.Invoke(_currentState);
             HandleSpawningEnemies(CancellationToken.None).Forget();
         }

         private async UniTask HandleSpawningEnemies(CancellationToken token)
         {
             var enemyWave = portalCharges[_currentPortalChargeIndex];

             await Spawner.SpawnEnemiesDuringTimeAsync(enemyWave.SpawnPoints, enemyWave.EnemyPrefabs, sceneContext,
                 enemyWave.WaveDurationSeconds, enemyWave.MaxTimeBetweenSpawnMilliseconds,
                 enemyWave.MinTimeBetweenSpawnMilliseconds, enemyWave.MaxEnemySpawnAtOnce, true, token);
             
             ClearAllSpawnsImmediate();
             await UniTask.Delay(changeStateDelayMilliseconds, cancellationToken: token);
             _currentState = States.Chill;
             OnStateChanged?.Invoke(_currentState);
         }
         
         private void ClearAllSpawnsImmediate()
         {
             foreach (var spawn in allSpawns)
             {
                 var childCount = spawn.childCount;
                 for (int i = childCount - 1; i >= 0; i--)
                 {
                     DestroyImmediate(spawn.GetChild(i).gameObject);
                 }
             }
         }
     }
}