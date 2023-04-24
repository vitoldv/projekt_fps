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

        protected float roundsPerMinute = 300f;
        protected float reloadTime = 1.5f;
        protected int gunShopCapacity = 30;
        protected PlayerController playerController;
        protected Camera camera;

        protected bool isReloading;
        protected int shotsMade = 0;
        protected float timeSinceLastShot = 0;
        protected float timeBetweenShots;

        public ShootingHandlerBase(ShootingParametersBase shootingParameters)
        {
            this.roundsPerMinute = shootingParameters.roundsPerMinute;
            this.reloadTime = shootingParameters.reloadTime;
            this.gunShopCapacity = shootingParameters.gunShopCapacity;
            this.playerController = shootingParameters.playerController;
            this.camera = shootingParameters.camera;
            timeBetweenShots = 60f / roundsPerMinute;
        }

        public void Update()
        {
            if (Input.GetMouseButton(0) && !isReloading && timeSinceLastShot >= timeBetweenShots)
            {                
                Shoot();
                shotsMade++;
                timeSinceLastShot = 0f;
            }

            timeSinceLastShot += Time.deltaTime;

            if ((shotsMade == gunShopCapacity || Input.GetKeyDown(KeyCode.R)) && !isReloading)
            {
                playerController.StartCoroutine(Reload());
            }
        }

        private IEnumerator Reload()
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);

            isReloading = false;
            shotsMade = 0;
        }

        protected virtual void Shoot() { }
    }
}
