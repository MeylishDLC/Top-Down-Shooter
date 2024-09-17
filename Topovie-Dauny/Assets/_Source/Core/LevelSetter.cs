using System;
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
         public event Action<States> OnStateChanged;
         public event Action<float, float> OnTimeRemainingChanged;
         public bool IsChargingPaused { get; private set; }
         
         [SerializeField] private SceneContext sceneContext;
         [SerializeField] private ShopTrigger shopTrigger;
         [SerializeField] private RangeDetector rangeDetector;
         [SerializeField] private Transform[] allSpawns;
         [SerializeField] private EnemyWave[] portalCharges;

         [Header("Time Settings")] 
         [SerializeField] private int changeStateDelayMilliseconds;

         private States _currentState = States.Chill;
         private int _currentPortalChargeIndex;
         
         private CancellationTokenSource _chargingPauseCts = new();
         private CancellationTokenSource _chargingFinishCts = new();
         private float _timeRemaining;
         private void Start()
         {
             shopTrigger.OnChargePortalPressed += StartChargingPortal;
             rangeDetector.OnPlayerEnterRange += ResumeChargingPortal;
             rangeDetector.OnPlayerExitRange += PauseChargingPortal;
         }
         private void OnDestroy()
         {
             shopTrigger.OnChargePortalPressed -= StartChargingPortal;
             rangeDetector.OnPlayerEnterRange -= ResumeChargingPortal;
             rangeDetector.OnPlayerExitRange -= PauseChargingPortal;
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

                 var enemyWave = portalCharges[_currentPortalChargeIndex];
                 StartSpawningEnemies(enemyWave);
                 ChargePortalAsync(enemyWave.TimeToActivatePencil, _chargingPauseCts.Token).Forget();
                 
                 StartTimeTrackingAsync(enemyWave.TimeToActivatePencil, enemyWave.TimeToActivatePencil
                     ,_chargingPauseCts.Token).Forget();
             }
             else
             {
                 Debug.Log("Level passed");
             }
         }
         private async UniTask ChargePortalAsync(float durationToCharge, CancellationToken token)
         {
             await UniTask.Delay(TimeSpan.FromSeconds(durationToCharge), cancellationToken: token);

             if (!token.IsCancellationRequested)
             {
                 _chargingFinishCts.Cancel();
                 _chargingFinishCts.Dispose();
                 _chargingFinishCts = new();
             
                 await EndWave(CancellationToken.None);
             }
         }
         private void PauseChargingPortal()
         {
             if (!IsChargingPaused)
             {
                 IsChargingPaused = true;
                 
                 _chargingPauseCts?.Cancel();
                 _chargingPauseCts?.Dispose();
                 Debug.Log($"Charge paused. Time Remaining: {_timeRemaining}");
             }
         }

         private void ResumeChargingPortal()
         {
             if (IsChargingPaused)
             {
                 IsChargingPaused = false;
                 _chargingPauseCts = new CancellationTokenSource();
                 
                 ChargePortalAsync(_timeRemaining, _chargingPauseCts.Token).Forget();
                 StartTimeTrackingAsync(_timeRemaining, 
                     portalCharges[_currentPortalChargeIndex].TimeToActivatePencil,_chargingPauseCts.Token).Forget();
             }
         }
         private async UniTask StartTimeTrackingAsync(float remainedDuration, float initialDuration, CancellationToken token)
         {
             var startTime = Time.time;
             _timeRemaining = remainedDuration;
             while (_timeRemaining > 0)
             {
                 if (token.IsCancellationRequested)
                 {
                     break;
                 }
                 _timeRemaining = remainedDuration - (Time.time - startTime);
                 OnTimeRemainingChanged?.Invoke(_timeRemaining, initialDuration);
                 await UniTask.Yield(PlayerLoopTiming.TimeUpdate);
             }
         }
         private void StartSpawningEnemies(EnemyWave enemyWave)
         {
             Spawner.SpawnEnemiesRandomlyAsync(enemyWave.SpawnPoints, enemyWave.EnemyPrefabs, sceneContext,
                 enemyWave.MaxTimeBetweenSpawnMilliseconds, enemyWave.MinTimeBetweenSpawnMilliseconds,
                 enemyWave.MaxEnemySpawnAtOnce, enemyWave.RandomizeEnemySpawnAmount, _chargingFinishCts.Token).Forget();
         }
         private async UniTask EndWave(CancellationToken token)
         {
             ClearAllSpawnsImmediate();

             await UniTask.Delay(changeStateDelayMilliseconds, cancellationToken: token);
             _currentPortalChargeIndex++;
             
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