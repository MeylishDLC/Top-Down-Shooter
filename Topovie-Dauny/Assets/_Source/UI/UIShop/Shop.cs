using UnityEngine;
using UnityEngine.UI;

namespace UI.UIShop
{
    public class Shop: MonoBehaviour
    {
        public bool ShopIsOpen { get; private set; }
        [field: SerializeField] public ShopCell[] ShopCells { get; private set; } 
        
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject playerGUI;
        [SerializeField] private Button closeButton;
        private void Start()
        {
            shopUI.SetActive(false);
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