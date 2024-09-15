using System;
using TMPro;
using UnityEngine;

namespace PlayerGUI
{
    public class BulletsAmountDisplay : MonoBehaviour
    {
        private TMP_Text bulletsAmountText;

        private void Start()
        {
            bulletsAmountText = GetComponent<TMP_Text>();
            
        }

        private void RefreshBulletAmountText()
        {
            
        }
    }
}
