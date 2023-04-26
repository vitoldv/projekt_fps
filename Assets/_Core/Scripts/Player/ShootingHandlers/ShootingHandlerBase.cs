using Assets._Core.Scripts.Player.ShootingParameters;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Core.Scripts
{
    public abstract class ShootingHandlerBase
    {
        public event Action AmmoStateChanged;
        public event Action OnReloadStarted;

        public ShootingHandlerState State => state;

        private ShootingHandlerState state;


        protected float roundsPerMinute = 300f;
        protected float reloadTime = 1.5f;                
        protected float damage;
        protected PlayerController playerController;
        protected Camera camera;
        public bool isReloading;
        protected float timeSinceLastShot = 0;
        protected float timeBetweenShots;

        public ShootingHandlerBase(ShootingHandlerArgs shootingParameters)
        {
            this.roundsPerMinute = shootingParameters.roundsPerMinute;
            this.reloadTime = shootingParameters.reloadTime;
            this.state.GunShopCapacity = shootingParameters.gunShopCapacity;
            this.playerController = shootingParameters.playerController;
            this.damage = shootingParameters.damage;
            this.camera = shootingParameters.camera;
            timeBetweenShots = 60f / roundsPerMinute;
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
                playerController.StartCoroutine(Reload());
            }
        }

        private IEnumerator Reload()
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);
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
            isReloading = false;
        }

        public void FillAmmo(int ammoAmount)
        {
            state.CurrentAmmoAmountTotal += ammoAmount;
            AmmoStateChanged?.Invoke();
        }

        protected virtual void Shoot() { }
    }
}
