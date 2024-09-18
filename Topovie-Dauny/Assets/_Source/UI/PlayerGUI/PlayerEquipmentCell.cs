using Player.PlayerAbilities;
using UI.UIShop;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.PlayerGUI
{
    public class PlayerEquipmentCell: MonoBehaviour
    {
        public Ability CurrentAbility { get; private set; }

        [SerializeField] private KeyCode keyToUse;
        [SerializeField] private int cellIndex; 
        [SerializeField] private PlayerCellsInShop playerCellsInShop;
        [SerializeField] private Image abilityImage;
        private void Awake()
        {
            playerCellsInShop.OnAbilityChanged += SwitchAbility;
        }
        private void Update()
        {
            if (Input.GetKeyDown(keyToUse))
            {
                CurrentAbility.UseAbility();
            }
        }
        private void SwitchAbility(int abilityCellIndex, Ability newAbility)
        {
            if (abilityCellIndex == cellIndex)
            {
                CurrentAbility = newAbility;
                abilityImage.sprite = CurrentAbility.AbilityImage;
                Debug.Log("Ability switched");
            }
        }
        
    }
}