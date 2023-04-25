using Assets._Core.Scripts.Player.ShootingParameters;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Core.Scripts
{
    public abstract class ShootingHandlerBase
    {
        public event Action OnShot;
        public event Action OnReloadStarted;

        public int CurrentAmmoAmount { get; private set; }
        public int CurrentAmmoAmountTotal { get; private set; }
        public int GunShopCapacity = 30;

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
            this.GunShopCapacity = shootingParameters.gunShopCapacity;
            this.playerController = shootingParameters.playerController;
            this.damage = shootingParameters.damage;
            this.camera = shootingParameters.camera;
            timeBetweenShots = 60f / roundsPerMinute;
            this.CurrentAmmoAmount = GunShopCapacity;
            this.CurrentAmmoAmountTotal = shootingParameters.initialAmmoAmount - GunShopCapacity;
        }

        public void Update()
        {
            if (Input.GetMouseButton(0) && !isReloading && CurrentAmmoAmount > 0 && timeSinceLastShot >= timeBetweenShots)
            {                
                Shoot();
                CurrentAmmoAmount--;
                timeSinceLastShot = 0f;
            }

            timeSinceLastShot += Time.deltaTime;

            // if player ran out of ammo and there is some carried ammo or if reload button was pressed and current ammo is less then gunshop capacity
            if (((CurrentAmmoAmount == 0 && CurrentAmmoAmountTotal > 0) || (Input.GetKeyDown(KeyCode.R) && CurrentAmmoAmount < GunShopCapacity)) && !isReloading)
            {
                playerController.StartCoroutine(Reload());
            }
        }

        private IEnumerator Reload()
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);
            if(CurrentAmmoAmount == 0)
            {
                if (CurrentAmmoAmountTotal <= GunShopCapacity)
                {
                    CurrentAmmoAmount = CurrentAmmoAmountTotal;
                    CurrentAmmoAmountTotal = 0;
                }
                else
                {
                    CurrentAmmoAmountTotal -= GunShopCapacity;
                    CurrentAmmoAmount += GunShopCapacity;
                }
            }
            else
            {
                var needToLoad = GunShopCapacity - CurrentAmmoAmount;
                if(CurrentAmmoAmountTotal > needToLoad)
                {
                    CurrentAmmoAmountTotal -= needToLoad;
                    CurrentAmmoAmount = GunShopCapacity;
                }
                else
                {
                    CurrentAmmoAmount += CurrentAmmoAmountTotal;
                    CurrentAmmoAmountTotal = 0;
                }
            }
            

            isReloading = false;
        }

        public void FillAmmo(int ammoAmount)
        {
            CurrentAmmoAmountTotal += ammoAmount;
        }

        protected virtual void Shoot() { }
    }
}
