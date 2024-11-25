using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player.PlayerCombat;
using Player.PlayerControl;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace UI.PlayerGUI
{
    public class UIHealthDisplay: MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        
        [Header("Heart Pounding")]
        [SerializeField] private RectTransform heartTransform;
        [SerializeField] private float minScale;
        [SerializeField] private float scaleDuration;
        
        [Header("Health floating")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private GameObject healTextPrefab;
        [SerializeField] private Canvas gameCanvas;

        private CancellationToken _deathCancellationToken;
        private PlayerHealth _playerHealth;
        private bool _heartIsAnimated;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        private void Awake()
        {
            _deathCancellationToken = _playerHealth.GetCancellationTokenOnDestroy();
            _playerHealth.OnDamageTaken += InstantiateDamageText;
            _playerHealth.OnHeal += InstantiateHealText;
            _playerHealth.OnDamageTaken += AnimateHeart;
        }
        private void Start()
        {
            UpdateSliderValue();
        }
        private void OnDestroy()
        {
            _playerHealth.OnDamageTaken -= InstantiateDamageText;
            _playerHealth.OnHeal -= InstantiateHealText;
            _playerHealth.OnDamageTaken -= AnimateHeart;
        }
        private void Update()
        {
            UpdateSliderValue();
        }

        private void InstantiateDamageText(float damage)
        {
            var playerPos = _playerHealth.transform.position;
            
            var spawnPosition = new Vector3 (playerPos.x, playerPos.y, 0);

            var tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

            tmpText.text = $"-{damage.ToString(CultureInfo.InvariantCulture)}";
        }

        private void InstantiateHealText(float heal)
        {
            var playerPos = _playerHealth.transform.position;

            var spawnPosition = new Vector3(playerPos.x, playerPos.y, 0);

            var tmpText = Instantiate(healTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
                .GetComponent<TMP_Text>();

            tmpText.text = $"-{heal.ToString(CultureInfo.InvariantCulture)}";
        }

        private void UpdateSliderValue()
        {
            var currentHealth = _playerHealth.CurrentHealth;
            var maxHealth = _playerHealth.MaxHealth;
            healthSlider.value = currentHealth / maxHealth;
        }
        private void AnimateHeart(float _)
        {
            if (!_heartIsAnimated)
            {
                AnimateHeartAsync(_deathCancellationToken).Forget();
            }
        } 
        private async UniTask AnimateHeartAsync(CancellationToken token)
        {
            _heartIsAnimated = true;
            await heartTransform.DOScale(minScale, scaleDuration).SetLoops(2, LoopType.Yoyo).ToUniTask(cancellationToken: token);
            _heartIsAnimated = false;
        }
    }
}