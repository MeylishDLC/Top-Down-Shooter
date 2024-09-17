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
         [SerializeField] private RangeDetector rangeDetector;
         [SerializeField] private Transform[] allSpawns;
         [SerializeField] private EnemyWave[] portalCharges;

         [Header("Time Settings")] [SerializeField]
         private int changeStateDelayMilliseconds;

         private States _currentState = States.Chill;
         private int _currentPortalChargeIndex;
         
         private CancellationTokenSource _chargingStopCts = new();
         private bool _isChargingPaused;
         private float _timeRemaining;
         private void Start()
         {
             shopTrigger.OnChargePortalPressed += StartChargingPortal;
             rangeDetector.OnPlayerEnterRange += ResumeChargingPortal;
             rangeDetector.OnPlayerExitRange += PauseChargingPortal;
             Spawner.OnFightStateStartTime += StartTimeTracking;
         }
         private void OnDestroy()
         {
             shopTrigger.OnChargePortalPressed -= StartChargingPortal;
             rangeDetector.OnPlayerEnterRange -= ResumeChargingPortal;
             rangeDetector.OnPlayerExitRange -= PauseChargingPortal;
             Spawner.OnFightStateStartTime -= StartTimeTracking;
         }
         private void Update()
         {
             shopTrigger.gameObject.SetActive(_currentState == States.Chill);
         }

         private void StartChargingPortal()
         {
             if (_currentPortalChargeIndex < portalCharges.Length)
             {
                 _currentState = States.Fight;
                 OnStateChanged?.Invoke(_currentState);
                 HandleSpawningEnemies(portalCharges[_currentPortalChargeIndex].WaveDurationSeconds, 
                     _chargingStopCts.Token).Forget();
             }
             else
             {
                 Debug.Log("Level passed");
             }
         }

         private async UniTask HandleSpawningEnemies(float duration, CancellationToken token)
         {
             var enemyWave = portalCharges[_currentPortalChargeIndex];
             
             await Spawner.SpawnEnemiesDuringTimeAsync(enemyWave.SpawnPoints, enemyWave.EnemyPrefabs, sceneContext,
                 duration, enemyWave.MaxTimeBetweenSpawnMilliseconds,
                 enemyWave.MinTimeBetweenSpawnMilliseconds, enemyWave.MaxEnemySpawnAtOnce, true, token);

             ClearAllSpawnsImmediate();
             
             await UniTask.Delay(changeStateDelayMilliseconds, cancellationToken: token);
             _currentPortalChargeIndex++;
             _currentState = States.Chill;
             OnStateChanged?.Invoke(_currentState);
         }
         private void PauseChargingPortal()
         {
             if (!_isChargingPaused)
             {
                 _isChargingPaused = true;
                 _chargingStopCts?.Cancel();
                 _chargingStopCts?.Dispose();
             }
         }

         private void ResumeChargingPortal()
         {
             if (_isChargingPaused)
             {
                 _isChargingPaused = false;
                 _chargingStopCts = new CancellationTokenSource();
                 HandleSpawningEnemies(_timeRemaining, _chargingStopCts.Token).Forget();
             }
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

         private void StartTimeTracking(float startTime, float duration)
         {
             StartTimeTrackingAsync(startTime, duration, CancellationToken.None).Forget();
         }
         
         private async UniTask StartTimeTrackingAsync(float startTime, float duration, CancellationToken token)
         {
             _timeRemaining = duration;
             while (Time.time - startTime < duration)
             {
                 _timeRemaining = duration - (Time.time - startTime);
                 await UniTask.Yield();
                 if (_isChargingPaused)
                 {
                     break;
                 }
             }

             if (!_isChargingPaused)
             {
                 _timeRemaining = 0;
             }
         }
     }
}