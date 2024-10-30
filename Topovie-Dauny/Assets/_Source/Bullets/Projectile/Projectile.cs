﻿using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using UnityEngine;

namespace Bullets.Projectile
{
    public class Projectile : MonoBehaviour
    {
        [field:SerializeField] public float Lifetime {get; private set;}
        [SerializeField] private ProjectileVisual projectileVisual;
        
        private readonly ProjectileCalculations _calculations = new();
        private void Awake()
        {
            Destroy(gameObject, Lifetime);
        }
        private void Update()
        {
            UpdatePosition();
        }
        public void Initialize(Transform target, ProjectileConfig config)
        {
            _calculations.Initialize(target, transform, config, projectileVisual);
        }
        private void UpdatePosition()
        {
            _calculations.UpdateProjectilePosition(transform);
        }
       
    }

}