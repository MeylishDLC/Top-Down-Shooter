using System;
using Core.LevelSettings;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace GameEnvironment.LevelObjects
{
    public class Portal: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer portalVisual;
        [SerializeField] private Light2D portalLight;
        [SerializeField] private Collider2D rangeTrigger;
        [SerializeField] private SpriteRenderer textBubble;

        private LevelChargesHandler _levelChargesHandler;

        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler)
        {
            _levelChargesHandler = levelChargesHandler;
            _levelChargesHandler.OnStateChanged += ActivateOnChangeState;
        }
        private void Awake()
        {
            DeactivatePortal();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                ShowTextBubble(true);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                ShowTextBubble(false);
            }
        }
        private void ActivateOnChangeState(GameStates state)
        {
            if (state == GameStates.PortalCharged)
            {
                ActivatePortal();
            }
        }
        private void ActivatePortal()
        {
            portalLight.gameObject.SetActive(true);
            rangeTrigger.enabled = true;
        }
        private void DeactivatePortal()
        {
            rangeTrigger.enabled = false;
            textBubble.gameObject.SetActive(false);
            portalLight.gameObject.SetActive(false);
        }
        private void ShowTextBubble(bool show)
        {
            textBubble.gameObject.SetActive(show);
        }
    }
}