using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Core
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        [SerializeField] private float smoothTime = 0.1f;

        [SerializeField] private SpriteRenderer defaultCrosshair;
        [SerializeField] private SpriteRenderer aimCrosshair;
        
        private Vector2 _targetPos;
        private Vector2 _currentPos;
        private Vector2 _velocity;
        private void Awake()
        {
            Cursor.visible = false;
            _currentPos = transform.position; 
        }

        private void Update()
        {
            _targetPos = cam.ScreenToWorldPoint(Input.mousePosition);
            _currentPos = Vector2.SmoothDamp(_currentPos, _targetPos, ref _velocity, smoothTime);
            transform.position = _currentPos;
        }

        public void SetCrosshair(bool aim)
        {
            if (aim)
            {
                aimCrosshair.gameObject.SetActive(true);
                defaultCrosshair.gameObject.SetActive(false);
            }
            else
            {
                aimCrosshair.gameObject.SetActive(false);
                defaultCrosshair.gameObject.SetActive(true);
            }
        }
    }
}
