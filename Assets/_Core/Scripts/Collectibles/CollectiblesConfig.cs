using _Core;
using _Core.Collectibles;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectiblesConfig", menuName = "Collectibles/Collectibles Config", order = 1)]
public class CollectiblesConfig : ScriptableObject
{
    [Header("Health")]
    public int bigHealthPack;
    public int midHealthPack;
    public int smallHealthPack;
    [Header("Ammo")]
    [Header("Pistol")]
    public int pistolSmallAmmoPack;
    public int pistolMidAmmoPack;
    public int pistolBigAmmoPack;
    [Header("Rifle")]
    public int rifleSmallAmmoPack;
    public int rifleMidAmmoPack;
    public int rifleBigAmmoPack;
    [Header("Shotgun")]
    public int shotgunSmallAmmoPack;
    public int shotgunMidAmmoPack;
    public int shotgunBigAmmoPack;
    [Header("BFG")]
    public int bfgSmallAmmoPack;
    public int bfgMidAmmoPack;
    public int bfgBigAmmoPack;
    [Header("Railgun")]
    public int railgunSmallAmmoPack;
    public int railgunMidAmmoPack;
    public int railgunBigAmmoPack;

    public int GetHealthPackSize(HealthCollectible.Size size)
    {
        switch (size)
        {
            case HealthCollectible.Size.BIG:
                return bigHealthPack;
            case HealthCollectible.Size.MID:
                return midHealthPack;
            case HealthCollectible.Size.SMALL:
                return smallHealthPack;
            default:
                return 0;
        }
    }

    public int[] GetAmmoPackSizesForWeapon(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.Pistol: 
                return new[] { pistolSmallAmmoPack, pistolMidAmmoPack, pistolBigAmmoPack };
            case WeaponType.Rifle:
                return new[] { rifleSmallAmmoPack, rifleMidAmmoPack, rifleBigAmmoPack };
            case WeaponType.Shotgun:
                return new[] { shotgunSmallAmmoPack, shotgunMidAmmoPack, shotgunBigAmmoPack };
            case WeaponType.BFG:
                return new[] { bfgSmallAmmoPack, bfgMidAmmoPack, bfgBigAmmoPack };
            case WeaponType.Railgun:
                return new[] { railgunSmallAmmoPack, railgunMidAmmoPack, railgunBigAmmoPack };
            default:
                return null;
        }
    }
}
