using Cinemachine;
using UnityEngine;
using Zenject;

namespace UI.Core
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField] private float smoothFactor = 10f;
        [SerializeField] private SpriteRenderer defaultCrosshair;
        [SerializeField] private SpriteRenderer aimCrosshair;

        private Vector2 _targetPos;
        private Camera _mainCamera;
        
        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }
        private void Awake()
        {
            Cursor.visible = false;

            CinemachineCore.CameraUpdatedEvent.AddListener(OnCinemachineCameraUpdated);
        }
        private void OnDestroy()
        {
            CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCinemachineCameraUpdated);
        }
        private void OnCinemachineCameraUpdated(CinemachineBrain brain)
        {
            _targetPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            transform.position = Vector2.Lerp(transform.position, _targetPos, Time.deltaTime * smoothFactor);
        }

        public void SetCrosshair(bool aim)
        {
            aimCrosshair.gameObject.SetActive(aim);
            defaultCrosshair.gameObject.SetActive(!aim);
        }
    }
}
