using Player.PlayerControl.GunMovement;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using Weapons.Guns;

namespace Player.PlayerCombat
{
    [System.Serializable]
    public class WeaponsSetterConfig
    {
        [field: SerializeField] public SerializedDictionary<int, Gun> Guns {get; private set;}
        [field: SerializeField] public GunRotation GunRotation  {get; private set;}
        [field: SerializeField] public Image GunUIImage  {get; private set;}
        [field: SerializeField] public Sprite GunKeyEnabled  {get; private set;}
        [field: SerializeField] public Sprite GunKeyDisabled  {get; private set;}
    }
}