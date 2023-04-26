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

    [SerializeField] private HUD hud;

    protected override void Initialize()
    {
        GameLayers = layers;
        CurrentWeaponConfiguration = DefaultWeaponConfiguration;

        // HUD creation
        hud.Init(PlayerController);
        hud.Show();
    }
}
