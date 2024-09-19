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

         [SerializeField] private PortalChargerTrigger[] portalChargeTriggers;
         
         [SerializeField] private SceneContext sceneContext;
         [SerializeField] private ShopTrigger shopTrigger;
         [SerializeField] private Transform[] allSpawns;
         [SerializeField] private EnemyWave[] portalCharges;

         [Header("Time Settings")] 
         [SerializeField] private int changeStateDelayMilliseconds;

         private States _currentState = States.Chill;
         
         private CancellationTokenSource _chargingPauseCts = new();
         private CancellationTokenSource _chargingFinishCts = new();
         private float _timeRemaining;
         
         private int _chargesPassed;
         private int _currentPortalIndex;
         private void Start()
         {
             SubscribeOnEvents(true);
         }
         private void OnDestroy()
         {
             SubscribeOnEvents(false);
         }
         private void Update()
         {
             shopTrigger.gameObject.SetActive(_currentState == States.Chill);
         }

         private void StartChargingPortal(int chargeIndex)
         {
             if (_chargesPassed < portalCharges.Length)
             {
                 _currentState = States.Fight;
                 _currentPortalIndex = chargeIndex;
                 
                 OnStateChanged?.Invoke(_currentState);
                 
                 var enemyWave = portalCharges[chargeIndex];
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
                 _chargingPauseCts = new CancellationTokenSource();
                 Debug.Log("Charge Paused");
             }
         }

         private void ResumeChargingPortal()
         {
             if (IsChargingPaused)
             {
                 IsChargingPaused = false;
                 
                 ChargePortalAsync(_timeRemaining, _chargingPauseCts.Token).Forget();
                 StartTimeTrackingAsync(_timeRemaining, 
                     portalCharges[_currentPortalIndex].TimeToActivatePencil,_chargingPauseCts.Token).Forget();
                 Debug.Log("Charge Resumed");
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
                 Debug.Log($"Time remained {_timeRemaining}, init duration {initialDuration}");
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
             _currentPortalIndex = default;
             
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

         private void SubscribeOnEvents(bool subscribe)
         {
             if (subscribe)
             {
                 foreach (var trigger in portalChargeTriggers)
                 {
                     trigger.OnChargePortalPressed += StartChargingPortal;

                     var rangeDetector = trigger.GetComponent<RangeDetector>();
                     rangeDetector.OnPlayerEnterRange += ResumeChargingPortal;
                     rangeDetector.OnPlayerExitRange += PauseChargingPortal;
                 }
             }
             else
             {
                 foreach (var trigger in portalChargeTriggers)
                 {
                     trigger.OnChargePortalPressed -= StartChargingPortal;
                     var rangeDetector = trigger.GetComponent<RangeDetector>();
                     rangeDetector.OnPlayerEnterRange -= ResumeChargingPortal;
                     rangeDetector.OnPlayerExitRange -= PauseChargingPortal;
                 }
             }
         }
     }
}