using Player.PlayerAbilities;
using TMPro;
using UnityEngine;

namespace UI.UIShop
{
    public class InfoPanel: MonoBehaviour
    {
        [SerializeField] private TMP_Text definitionText;
        [SerializeField] private TMP_Text descriptionText;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ChangeInfo(Ability ability)
        {
            definitionText.text = ability.AbilityDefinition;
            descriptionText.text = ability.AbilityDescription;
        }
    }
}