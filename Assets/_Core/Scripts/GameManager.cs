using UnityEngine;
using _Core;
using Assets._Core.Scripts;

public class GameManager : Singleton<GameManager>
{
    public static Layers GameLayers;

    public Layers layers;

    public WeaponsConfiguration DefaultWeaponConfiguration;
    public static WeaponsConfiguration CurrentWeaponConfiguration { get; private set; }
    public PlayerController PlayerController;

    protected override void Initialize()
    {
        GameLayers = layers;
        CurrentWeaponConfiguration = DefaultWeaponConfiguration;
        PlayerController.WeaponChanged += OnWeaponChanged;
    }

    private void OnWeaponChanged(WeaponType weapon)
    {
        // update UI
        Debug.Log($"Weapon changed to: {weapon}");
    }
}
