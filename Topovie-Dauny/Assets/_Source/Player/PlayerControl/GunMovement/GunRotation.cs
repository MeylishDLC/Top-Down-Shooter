using System;
using DialogueSystem;
using UI.UIShop;
using UnityEngine;
using Zenject;

namespace Player.PlayerControl.GunMovement
{
    public class GunRotation: MonoBehaviour
    { 
        public SpriteRenderer CurrentGun { get; set; }
        
        [SerializeField] private GameObject front; 
        [SerializeField] private GameObject left; 
        [SerializeField] private GameObject right;
        [SerializeField] private GameObject back;
        
        private Shop _shop;
        private DialogueManager _dialogueManager;
        
        [Inject]
        public void Construct(DialogueManager dialogueManager, Shop shop)
        {
            _dialogueManager = dialogueManager;
            _shop = shop;
        }
        private void FixedUpdate()
        {
            if (_shop.IsShopOpen() || _dialogueManager.DialogueIsPlaying)
            {
                return;
            }
            
            var difference = CountDifference();
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            
            if (CheckRolling.IsRolling == false)
            {
                if (transform.eulerAngles.z >= 0 && transform.eulerAngles.z <= 44)
                {
                    SetOneSideActive(Sides.Right);
                }
                if (transform.eulerAngles.z >= 130 && transform.eulerAngles.z <= 180)
                {
                    SetOneSideActive(Sides.Left);
                }
                if (transform.eulerAngles.z >= 44 && transform.eulerAngles.z <= 130)
                {
                    SetOneSideActive(Sides.Back);
                }
                if (transform.eulerAngles.z >= 220 && transform.eulerAngles.z <= 340)
                {
                    SetOneSideActive(Sides.Front);
                }
            }
        }

        private void SetOneSideActive(Sides side)
        {
            switch (side)
            {
                case Sides.Front:
                    front.SetActive(true);
                    left.SetActive(false);
                    right.SetActive(false);
                    back.SetActive(false);
                    CurrentGun.sortingOrder = 1;
                    
                    break;
                
                case Sides.Right:
                    front.SetActive(false);
                    left.SetActive(false);
                    right.SetActive(true);
                    back.SetActive(false);
                    
                    FlipGameObject(CurrentGun.gameObject, false);
                    CurrentGun.sortingOrder = 1;
                    
                    break;
                
                case Sides.Left:
                    front.SetActive(false);
                    left.SetActive(true);
                    right.SetActive(false);
                    back.SetActive(false);
                    FlipGameObject(CurrentGun.gameObject, true); 
                    CurrentGun.sortingOrder = 1;
                    
                    break;
                
                case Sides.Back:
                    front.SetActive(false);
                    left.SetActive(false);
                    right.SetActive(false);
                    back.SetActive(true);
                    CurrentGun.sortingOrder = -1;
                    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
        private Vector3 CountDifference()
        {
            var difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            return difference;
        }
        private void FlipGameObject(GameObject obj, bool flipY)
        {
            var scale = obj.transform.localScale;
            scale.y = flipY ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            obj.transform.localScale = scale;
        }
        
        private enum Sides
        {
            Front,
            Right,
            Left,
            Back
        }
    }
}