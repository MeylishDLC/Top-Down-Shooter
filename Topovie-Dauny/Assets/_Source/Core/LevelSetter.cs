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
                [SerializeField] private SceneContext sceneContext;
                [SerializeField] private ShopTrigger shopTrigger;
                [SerializeField] private SerializedDictionary<int, EnemyWave[]> portalChargingsEnemyWavesPair;
                
                private States _currentState = States.Chill;

                private enum States
                {
                    Fight,
                    Chill
                }
            }
}