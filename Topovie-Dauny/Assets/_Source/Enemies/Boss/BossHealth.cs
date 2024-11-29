using System;
using Cysharp.Threading.Tasks;
using Enemies.Boss.Phases;
using UnityEngine;

namespace Enemies.Boss
{
    public class BossHealth: MonoBehaviour, IEnemyHealth
    {
        public event Action OnPhaseFinished;

        private int _maxHealth;
        private int _currentHealth;
        
        private IBossPhase _currentPhase;
        private bool _isVulnerable;

        public void ChangePhase(IBossPhase phase)
        {
            _currentPhase = phase;
            _currentPhase.OnPhaseStateChanged += SetVulnerability;
            
            _maxHealth = _currentPhase.PhaseConfig.BossHealth;
            _currentHealth = _maxHealth;
        }
        public void TakeDamage(int damage)
        {
            if (!_isVulnerable)
            {
                return;
            }
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                if (_currentPhase != null)
                {
                    OnHealthWasted();
                }
            }
        }
        
        private void OnHealthWasted()
        {
            _currentPhase.FinishPhase();
            _currentPhase.OnPhaseStateChanged -= SetVulnerability;
            _currentPhase = null;
            OnPhaseFinished?.Invoke();
        }
        private void SetVulnerability(PhaseState phaseState)
        {
            if (phaseState == PhaseState.Vulnerability)
            {
                _isVulnerable = true;
                ShowHealthBar();
            }
            else
            {
                _isVulnerable = false;
                HideHealthBar();
            }
        }
        private void ShowHealthBar()
        {
            Debug.Log("Showing health bar");
        }
        private void HideHealthBar()
        {
            Debug.Log("Hiding health bar");
        }
    }
}