using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cinemachine;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using Player.PlayerControl;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Core
{
    public class ArrowIndicator: MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform target; 
        [SerializeField] private RectTransform arrowUI;
        [SerializeField] private Image arrowImage;
        [SerializeField] private float hideDistance;
        [SerializeField] private float fadeDuration;
        [SerializeField] private float smoothFactor = 10f;

        private Camera _mainCam;
        private Transform _player; 
        private RectTransform _canvasRect;
        private StatesChanger _statesChanger;
        private bool _isFaded;
        private CancellationToken _destroyCancellationToken;

        [Inject]
        public void Construct(Camera mainCam, PlayerMovement playerMovement, StatesChanger statesChanger)
        {
            _mainCam = mainCam;
            _player = playerMovement.transform;
            _statesChanger = statesChanger;
        }
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _canvasRect = canvas.GetComponent<RectTransform>();
            CinemachineCore.CameraUpdatedEvent.AddListener(OnCinemachineCameraUpdated);
            gameObject.SetActive(false);
            _statesChanger.OnStateChanged += EnableOnStateChange;
        }
        private void OnDestroy()
        {
            CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCinemachineCameraUpdated);
            _statesChanger.OnStateChanged -= EnableOnStateChange;
        }
        private void EnableOnStateChange(GameStates state)
        {
            if (state == GameStates.PortalCharged)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        private void OnCinemachineCameraUpdated(CinemachineBrain brain)
        {
            if (IsCloseToTarget())
            {
                if (!_isFaded)
                {
                    Fade(0f, _destroyCancellationToken).Forget();
                    _isFaded = true;
                }
            }
            else
            {
                if (_isFaded)
                {
                    Fade(1f, _destroyCancellationToken).Forget();
                    _isFaded = false;
                }
            }
            SetPosition();
        }
        private void SetPosition()
        {
            var direction = target.position - _player.position;
            direction.z = 0;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.Euler(0, 0, angle);

            arrowUI.rotation = Quaternion.Lerp(arrowUI.rotation, targetRotation, Time.deltaTime * smoothFactor);

            var screenPosition = _mainCam.WorldToScreenPoint(_player.position + direction.normalized);
            screenPosition.x = Mathf.Clamp(screenPosition.x, 50, Screen.width - 50);
            screenPosition.y = Mathf.Clamp(screenPosition.y, 50, Screen.height - 50);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPosition, _mainCam, out var localPoint);

            arrowUI.localPosition = Vector3.Lerp(arrowUI.localPosition, localPoint, Time.deltaTime * smoothFactor);
        }
        private UniTask Fade(float value, CancellationToken token)
        {
            return arrowImage.DOFade(value, fadeDuration).ToUniTask(cancellationToken: token);
        }
        private bool IsCloseToTarget()
        {
            var distanceToTarget = Vector3.Distance(_player.position, target.position);
            if (distanceToTarget < hideDistance)
            {
                return true;
            }
            return false;
        }
    }
}