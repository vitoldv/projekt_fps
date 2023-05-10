using _Core;
using _Core.Common;
using _Core.Player;
using UnityEngine;

namespace _Core.Collectibles
{
    public class AmmoCollectible : CollectibleBase
    {
        public enum Size
        {
            BIG,
            MID,
            SMALL
        }

        [SerializeField] private float smallPackScale;
        [SerializeField] private float bigPackScale;
        [SerializeField] private int ammoAmount;
        [SerializeField] private WeaponType weaponType;
        // SOME RESOURCE TO DIFFERENTIATE ONE TYPE AMMO FROM ANOTHER
        // [SerializeField] private Texture emblem;

        public void Init(Size size, WeaponType weaponType)
        {
            var ammoAmounts = GameManager.CollectiblesConfig.GetAmmoPackSizesForWeapon(weaponType);
            switch (size)
            {   
                case Size.BIG:
                    ammoAmount = ammoAmounts[2];
                    transform.localScale = new Vector3(bigPackScale, bigPackScale, bigPackScale);
                    break;
                case Size.MID:
                    ammoAmount = ammoAmounts[1];
                    transform.localScale = Vector3.one;
                    break;
                case Size.SMALL:
                    ammoAmount = ammoAmounts[0];
                    transform.localScale = new Vector3(smallPackScale, smallPackScale, smallPackScale);
                    break;
                default:
                    break;
            }
            this.weaponType = weaponType;
        }

        protected override void ApplyEffect(PlayerController player)
        {
            player.GetAmmo(ammoAmount, weaponType);
        }
    }
}