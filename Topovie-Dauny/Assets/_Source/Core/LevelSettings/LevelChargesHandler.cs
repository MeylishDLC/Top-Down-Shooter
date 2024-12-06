﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Data;
using Cysharp.Threading.Tasks;
using Enemies;
using GameEnvironment;
using SoundSystem;
using UI.Menus;
using UnityEngine;
using Zenject;

namespace Core.LevelSettings
{
     public class LevelChargesHandler: MonoBehaviour
     {
         public event Action OnChargePassed; 
         public event Action<float, float> OnTimeRemainingChanged;
         public int ChargesPassed { get; private set; }

         [Header("Main")]
         [SerializeField] private PortalChargerTrigger[] portalChargeTriggers;
         [SerializeField] private ShopTrigger shopTrigger;
         [SerializeField] private EnemyWave[] portalCharges;
         [SerializeField] private GameOverScreen gameOverScreen;

         [Header("Time Settings")] 
         [SerializeField] private int changeStateDelayMilliseconds;
         
         private EnemyContainer[] _allEnemyContainers;
         private Spawner _spawner;
         private LevelChargeSound _levelChargeSound;
         private StatesChanger _statesChanger;
         
         private CancellationTokenSource _chargingPauseCts = new();
         private CancellationTokenSource _chargingFinishCts = new();
         private float _timeRemaining;
         private int _currentChargeIndex;

         [Inject]
         public void Construct(Spawner spawner, StatesChanger statesChanger, AudioManager audioManager)
         {
             _statesChanger = statesChanger;
             _spawner = spawner;
             _levelChargeSound = new LevelChargeSound(audioManager);
         }
         private void Awake()
         {
             gameOverScreen.OnGameOver += StopSpawning;
             gameOverScreen.OnScreenFaded += DisableAllEnemies;
         }
         private void Start()
         {
             SubscribeOnStartCharging();
         }
         private void OnDestroy()
         {
             foreach (var trigger in portalChargeTriggers)
             {
                 UnsubscribeOnStartCharging(trigger);
             }
             gameOverScreen.OnGameOver -= StopSpawning;
             gameOverScreen.OnScreenFaded -= DisableAllEnemies;
         }
         public void InitAllContainers(IEnumerable<EnemyContainer> containers)
         {
             _allEnemyContainers = containers.ToArray();
         }
         private void StopSpawning()
         {
             _chargingFinishCts?.Cancel();
             _chargingFinishCts?.Dispose();
         }
         private void StartChargingPortal(int chargeIndex)
         {
             if (_statesChanger.CurrentGameState == GameStates.Fight)
             {
                 return;
             }
             if (ChargesPassed >= portalCharges.Length)
             {
                 return;
             }
             
             _currentChargeIndex = chargeIndex;
             UnsubscribeOnStartCharging(portalChargeTriggers[_currentChargeIndex]);
             SubscribeOnChargeEvents(portalChargeTriggers[_currentChargeIndex].GetComponent<RangeDetector>());
             _statesChanger.ChangeState(GameStates.Fight);
                 
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
                 _levelChargeSound.PlayChargeSound();
                 await UniTask.Delay(TimeSpan.FromSeconds(durationToCharge), cancellationToken: token);
                 _chargingFinishCts.Cancel();
                 _chargingFinishCts.Dispose();
                 _chargingFinishCts = new();
             
                 await EndWave(CancellationToken.None);
             }
             catch (OperationCanceledException)
             {
                 //
             }
         }
         private void PauseChargingPortal()
         {
             _chargingPauseCts?.Cancel();
             _chargingPauseCts?.Dispose();
             _chargingPauseCts = new CancellationTokenSource();
             
             _levelChargeSound.StopChargeSound();
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
             DisableAllEnemies();
             UnsubscribeOnChargeEvents(portalChargeTriggers[_currentChargeIndex].GetComponent<RangeDetector>());
             _levelChargeSound.StopChargeSound();
             portalChargeTriggers[_currentChargeIndex].enabled = false;
             OnChargePassed?.Invoke();
             try
             {
                 await UniTask.Delay(changeStateDelayMilliseconds, cancellationToken: token);
                 _currentChargeIndex = -1;
                 ChargesPassed += 1;

                 if (ChargesPassed >= portalCharges.Length)
                 {
                     _statesChanger.ChangeState(GameStates.PortalCharged);
                 }
                 else
                 {
                     _statesChanger.ChangeState(GameStates.Chill);
                 }
             }
             catch (OperationCanceledException)
             {
                 //
             }
         }
         private void DisableAllEnemies()
         {
             foreach (var pool in _allEnemyContainers.Select(p => p.Pool))
             {
                 pool.DisableAll();
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
         private void UnsubscribeOnStartCharging(PortalChargerTrigger trigger)
         {
             trigger.OnChargePortalPressed -= StartChargingPortal;
         }
        
     }
}