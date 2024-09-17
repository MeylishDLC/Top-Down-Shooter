using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player.PlayerCombat;
using Player.PlayerMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIHealthDisplay: MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        
        [Header("Heart Pounding")]
        [SerializeField] private Image heartImage;
        [SerializeField] private float minScale;
        [SerializeField] private float scaleDuration;
        
        [Header("Health floating")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Canvas gameCanvas;

        private PlayerHealth _playerHealth;
        private bool _heartIsAnimated;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        private void Awake()
        {
            _playerHealth.OnDamageTaken += InstantiateDamageText;
            _playerHealth.OnDamageTaken += AnimateHeart;
        }
        private void Start()
        {
            UpdateSliderValue();
        }
        private void OnDestroy()
        {
            _playerHealth.OnDamageTaken -= InstantiateDamageText;
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
                AnimateHeartAsync(CancellationToken.None).Forget();
            }
        }
        //todo: refactor (cts too)
        private async UniTask AnimateHeartAsync(CancellationToken token)
        {
            _heartIsAnimated = true;
            await heartImage.transform.DOScale(minScale, scaleDuration).SetLoops(2, LoopType.Yoyo).ToUniTask(cancellationToken: token);
            _heartIsAnimated = false;
        }
    }
}