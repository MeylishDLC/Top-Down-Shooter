using Player.PlayerControl.GunMovement;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player.PlayerCombat
{
    [System.Serializable]
    public class WeaponsSetterConfig
    {
        [field: SerializeField] public SerializedDictionary<int, Gun> Guns {get; private set;}
        [field: SerializeField] public GunRotation GunRotation  {get; private set;}
        [field: SerializeField] public Image GunUIImage  {get; private set;}
        [field: SerializeField] public Color GunKeyColorEnabled  {get; private set;}
        [field: SerializeField] public Color GunKeyColorDisabled  {get; private set;}
    }
}