using System;
using System.Threading;
using Core.EnemyWaveData;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WorldShop;
using Zenject;

namespace Core
{
     public class LevelChargesHandler: MonoBehaviour
     {
         public event Action<GameStates> OnStateChanged;
         public event Action<float, float> OnTimeRemainingChanged;
         
         [Header("Main")]
         [SerializeField] private PortalChargerTrigger[] portalChargeTriggers;
         [SerializeField] private ShopTrigger shopTrigger;
         [SerializeField] private Transform[] allSpawns;
         [SerializeField] private EnemyWave[] portalCharges;

         [Header("Time Settings")] 
         [SerializeField] private int changeStateDelayMilliseconds;
         
         private Spawner _spawner;
         private GameStates _currentGameState = GameStates.Chill;
         
         private CancellationTokenSource _chargingPauseCts = new();
         private CancellationTokenSource _chargingFinishCts = new();
         private float _timeRemaining;
         
         private int _chargesPassed;
         private int _currentChargeIndex;

         [Inject]
         public void Construct(Spawner spawner)
         {
             _spawner = spawner;
         }
         private void Start()
         {
             SubscribeOnStartCharging();
         }
         private void OnDestroy()
         {
             UnsubscribeOnStartCharging();
         }
         private void StartChargingPortal(int chargeIndex)
         {
             if (_chargesPassed >= portalCharges.Length)
             {
                 return;
             }
             
             _currentGameState = GameStates.Fight;
             _currentChargeIndex = chargeIndex;
             SubscribeOnChargeEvents(portalChargeTriggers[_currentChargeIndex].GetComponent<RangeDetector>());
                 
             OnStateChanged?.Invoke(_currentGameState);
                 
             var enemyWave = portalCharges[chargeIndex];
             StartSpawningEnemies(enemyWave);
             ChargePortal(enemyWave.TimeToActivatePencil, _chargingPauseCts.Token).Forget();
                 
             StartTimeTracking(enemyWave.TimeToActivatePencil, enemyWave.TimeToActivatePencil
                 ,_chargingPauseCts.Token).Forget();

         }
         private async UniTask ChargePortal(float durationToCharge, CancellationToken token)
         {
             try
             {
                 await UniTask.Delay(TimeSpan.FromSeconds(durationToCharge), cancellationToken: token);
                 _chargingFinishCts.Cancel();
                 _chargingFinishCts.Dispose();
                 _chargingFinishCts = new();
             
                 await EndWave(CancellationToken.None);
             }
             catch
             {
                 //ignored
             }
         }
         private void PauseChargingPortal()
         {
             _chargingPauseCts?.Cancel();
             _chargingPauseCts?.Dispose();
             _chargingPauseCts = new CancellationTokenSource();
             Debug.Log("Charge Paused");
         }
         private void ResumeChargingPortal()
         {
             ChargePortal(_timeRemaining, _chargingPauseCts.Token).Forget();
             StartTimeTracking(_timeRemaining, 
                 portalCharges[_currentChargeIndex].TimeToActivatePencil,_chargingPauseCts.Token).Forget();
             Debug.Log("Charge Resumed");
         }
         private async UniTask StartTimeTracking(float remainedDuration, float initialDuration, CancellationToken token)
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
             _spawner.InitializeEnemyWave(enemyWave);
             _spawner.SpawnEnemiesRandomlyAsync(_chargingFinishCts.Token).Forget();
         }
         private async UniTask EndWave(CancellationToken token)
         {
             ClearAllSpawnsImmediate();
             UnsubscribeOnChargeEvents(portalChargeTriggers[_currentChargeIndex].GetComponent<RangeDetector>());
             try
             {
                 await UniTask.Delay(changeStateDelayMilliseconds, cancellationToken: token);
                 _currentChargeIndex = -1;
             
                 _currentGameState = GameStates.Chill;
                 OnStateChanged?.Invoke(_currentGameState);
             }
             catch
             {
                 //ignored
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
         private void SubscribeOnChargeEvents(RangeDetector rangeDetector)
         {
             rangeDetector.OnPlayerEnterRange += ResumeChargingPortal;
             rangeDetector.OnPlayerExitRange += PauseChargingPortal;
         }
         private void UnsubscribeOnChargeEvents(RangeDetector rangeDetector)
         {
             rangeDetector.OnPlayerEnterRange -= ResumeChargingPortal;
             rangeDetector.OnPlayerExitRange -= PauseChargingPortal;
         }
         private void SubscribeOnStartCharging()
         {
             foreach (var trigger in portalChargeTriggers)
             {
                 trigger.OnChargePortalPressed += StartChargingPortal;
             }
         }
         private void UnsubscribeOnStartCharging()
         {
             foreach (var trigger in portalChargeTriggers)
             {
                 trigger.OnChargePortalPressed -= StartChargingPortal;
             }
         }
     }
}