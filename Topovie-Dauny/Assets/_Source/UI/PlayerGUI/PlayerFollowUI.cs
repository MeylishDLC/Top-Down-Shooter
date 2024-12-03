using System;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace UI.PlayerGUI
{
    public class PlayerFollowUI: MonoBehaviour
    {
        [SerializeField] private GameObject objectToFollowPlayer;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Vector2 padding;
        [SerializeField] private float smoothTime = 0.1f;

        private Camera _mainCamera;
        private Transform _playerTransform;
        private RectTransform _uiRectTransform;
        private Vector2 _velocity = Vector2.zero;

        [Inject]
        public void Construct(PlayerMovement playerMovement, Camera mainCamera)
        {
            _playerTransform = playerMovement.transform;
            _mainCamera = mainCamera;
        }
        private void Awake()
        {
            _uiRectTransform = objectToFollowPlayer.GetComponent<RectTransform>();
        }
        private void OnEnable()
        {
            MoveToPlayerInstantly();
        }
        private void Update()
        {
            if (_playerTransform)
            {
                FollowPlayerSmoothly();
            }
        }
        private void FollowPlayerSmoothly()
        {
            var canvasPosition = GetCanvasPosition();
            _uiRectTransform.anchoredPosition = Vector2.SmoothDamp(_uiRectTransform.anchoredPosition, 
                canvasPosition, ref _velocity, smoothTime);
        }
        private void MoveToPlayerInstantly()
        {
            _uiRectTransform.anchoredPosition = GetCanvasPosition();
        }
        private Vector2 GetCanvasPosition()
        {
            var playerScreenPosition = _mainCamera.WorldToScreenPoint(_playerTransform.position);

            var canvasRect = canvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, playerScreenPosition, _mainCamera, 
                out var canvasPosition);

            canvasPosition += padding;
            return canvasPosition;
        }
    }
}