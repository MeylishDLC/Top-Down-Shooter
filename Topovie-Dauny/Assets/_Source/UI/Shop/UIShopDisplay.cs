using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIShopDisplay: MonoBehaviour
    {
        public bool ShopIsOpen { get; private set; }
        
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject playerGUI;
        [SerializeField] private Button closeButton;
        private void Start()
        {
            closeButton.onClick.AddListener(CloseShop);
        }
        public void OpenShop()
        {
            ShopIsOpen = true;
            shopUI.SetActive(true);
            playerGUI.SetActive(false);
        }
        public void CloseShop()
        {
            ShopIsOpen = false;
            shopUI.SetActive(false);
            playerGUI.SetActive(true);
        }
    }
}