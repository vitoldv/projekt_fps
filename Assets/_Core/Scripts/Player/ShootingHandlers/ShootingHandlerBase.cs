using System;
using System.Collections;
using UnityEngine;

namespace _Core.Player
{
    public abstract class ShootingHandlerBase
    {
        public event Action AmmoStateChanged;
        public event Action OnReloadStarted;
        public event Action OnReloadEnded;

        public ShootingHandlerState State => state;

        private ShootingHandlerState state;

        protected PlayerController playerController;
        protected float timeSinceLastShot = 0;
        protected ShootingHandlerArgs baseParams;
        protected float timeBetweenShots;

        public bool isReloading;

        public ShootingHandlerBase(ShootingHandlerArgs shootingParameters)
        {
            baseParams = shootingParameters;
            //this.baseParams.roundsPerMinute = shootingParameters.roundsPerMinute;
            //this.baseParams.reloadTime = shootingParameters.reloadTime;
            this.state.GunShopCapacity = shootingParameters.gunShopCapacity;
            this.playerController = shootingParameters.playerController;
            //this.baseParams.damage = shootingParameters.damage;
            timeBetweenShots = 60f / shootingParameters.roundsPerMinute;
            this.state.CurrentAmmoAmount = state.GunShopCapacity;
            this.state.CurrentAmmoAmountTotal = shootingParameters.initialAmmoAmount - state.GunShopCapacity;
        }

        public void Update()
        {
            if (Input.GetMouseButton(0) && !isReloading && state.CurrentAmmoAmount > 0 && timeSinceLastShot >= timeBetweenShots)
            {                
                Shoot();
                state.CurrentAmmoAmount--;
                timeSinceLastShot = 0f;
                AmmoStateChanged?.Invoke();
            }

            timeSinceLastShot += Time.deltaTime;

            // if player ran out of ammo and there is some carried ammo or if reload button was pressed and current ammo is less then gunshop capacity
            if (((state.CurrentAmmoAmount == 0 && state.CurrentAmmoAmountTotal > 0) || (Input.GetKeyDown(KeyCode.R) && state.CurrentAmmoAmount < state.GunShopCapacity)) && !isReloading)
            {
                playerController.StartCoroutine(C_Reload());
            }
        }

        private IEnumerator C_Reload()
        {
            isReloading = true;
            OnReloadStarted?.Invoke();
            yield return new WaitForSeconds(baseParams.reloadTime);
            if(state.CurrentAmmoAmount == 0)
            {
                if (state.CurrentAmmoAmountTotal <= state.GunShopCapacity)
                {
                    state.CurrentAmmoAmount = state.CurrentAmmoAmountTotal;
                    state.CurrentAmmoAmountTotal = 0;
                }
                else
                {
                    state.CurrentAmmoAmountTotal -= state.GunShopCapacity;
                    state.CurrentAmmoAmount += state.GunShopCapacity;
                }
            }
            else
            {
                var needToLoad = state.GunShopCapacity - state.CurrentAmmoAmount;
                if(state.CurrentAmmoAmountTotal > needToLoad)
                {
                    state.CurrentAmmoAmountTotal -= needToLoad;
                    state.CurrentAmmoAmount = state.GunShopCapacity;
                }
                else
                {
                    state.CurrentAmmoAmount += state.CurrentAmmoAmountTotal;
                    state.CurrentAmmoAmountTotal = 0;
                }
            }

            AmmoStateChanged?.Invoke();
            OnReloadEnded?.Invoke();
            isReloading = false;
        }

        public void FillAmmo(int ammoAmount)
        {
            state.CurrentAmmoAmountTotal += ammoAmount;
            AmmoStateChanged?.Invoke();
        }

        public void SetUpgradedParams(float newReloadTimeValue, float newDamageValue, int newGunShopCapacityValue)
        {
            this.state.GunShopCapacity = newGunShopCapacityValue;
            this.baseParams.gunShopCapacity = newGunShopCapacityValue;
            this.baseParams.damage = newDamageValue;
            this.baseParams.reloadTime = newReloadTimeValue;
        }

        protected virtual void Shoot() { }
    }
}
