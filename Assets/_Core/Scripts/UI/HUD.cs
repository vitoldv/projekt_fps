using _Core;
using _Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.UI
{


    public class HUD : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private Texture pistolIconTexture;
        [SerializeField] private Texture rifleIconTexture;
        [SerializeField] private Texture shotgunIconTexture;
        [SerializeField] private Texture bfgIconTexture;
        [SerializeField] private Texture railgunIconTexture;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI hpAmount;
        [SerializeField] private TextMeshProUGUI ammoAmount;
        [SerializeField] private TextMeshProUGUI dashTimeLeft;
        [SerializeField] private TextMeshProUGUI quakeTimeLeft;
        [SerializeField] private RawImage selectedWeaponIcon;
        [SerializeField] private TextMeshProUGUI selectedWeaponName;

        private PlayerController player;

        public void Init(PlayerController player)
        {
            this.player = player;
            player.HealthValueChanged += OnPlayerHealthValueChanged;
            player.AmmoValuesChanged += OnAmmoValueChanged;
            player.DashCooldownValueChanged += OnDashCooldownValueChanged;
            player.QuakeCooldownValueChanged += OnQuakeCooldownValueChanged;
            player.WeaponSelected += OnPlayerWeaponSelected;
            player.PlayerDied += OnPlayerDeath;
            hpAmount.text = $"{player.CurrentHP}";
            OnPlayerWeaponSelected(player.SelectedWeapon, player.SelectedWeaponState);
        }

        private void OnPlayerHealthValueChanged(float newHealthValue)
        {
            hpAmount.text = $"{newHealthValue}";
        }

        private void OnAmmoValueChanged(ShootingHandlerState shootingHandlerState)
        {
            ammoAmount.text = $"{shootingHandlerState.CurrentAmmoAmount}/{shootingHandlerState.GunShopCapacity} {shootingHandlerState.CurrentAmmoAmountTotal}";
        }

        private void OnDashCooldownValueChanged(int dashCooldownValue)
        {
            dashTimeLeft.text = $"{dashCooldownValue}";
        }

        private void OnQuakeCooldownValueChanged(int quakeCooldownValue)
        {
            quakeTimeLeft.text = $"{quakeCooldownValue}";
        }

        private void OnPlayerWeaponSelected(WeaponType weapon, ShootingHandlerState shootingHandlerState)
        {
            Texture selectedTexture = null;
            switch (weapon)
            {
                case WeaponType.Pistol: selectedTexture = pistolIconTexture; break;
                case WeaponType.Rifle: selectedTexture = rifleIconTexture; break;
                case WeaponType.Shotgun: selectedTexture = shotgunIconTexture; break;
                case WeaponType.BFG: selectedTexture = bfgIconTexture; break;
                case WeaponType.Railgun: selectedTexture = railgunIconTexture; break;
            }
            selectedWeaponIcon.texture = selectedTexture;
            selectedWeaponName.text = weapon.ToString();
            OnAmmoValueChanged(shootingHandlerState);
        }

        private void OnPlayerDeath()
        {
            // TODO
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        //private void OnDestroy()
        //{
        //    player.HealthValueChanged -= OnPlayerHealthValueChanged;
        //    player.AmmoValuesChanged -= OnAmmoValueChanged;
        //    player.DashCooldownValueChanged -= OnDashCooldownValueChanged;
        //    player.QuakeCooldownValueChanged -= OnQuakeCooldownValueChanged;
        //}
    }
}
