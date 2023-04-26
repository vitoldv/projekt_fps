using System;

namespace _Core
{
    [Flags]
    public enum WeaponType
    {
        Pistol = 1,
        Rifle = 2,
        Shotgun = 4,
        BFG = 8,
        Railgun = 16
    }
}

