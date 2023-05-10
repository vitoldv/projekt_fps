using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using _Core.Saves;
using _Core.Common;

namespace _Core.Player
{
    public class PlayerController : MonoBehaviour
    {
        public event Action PlayerDied;
        // params: new selected weapon, ammoValue, ammoValueTotal, gunShopCapasityValue
        public event Action<WeaponType, ShootingHandlerState> WeaponSelected;
        // params: new health value
        public event Action<float> HealthValueChanged;
        // params: ammoValue, ammoValueTotal, gunShopCapasityValue
        public event Action<ShootingHandlerState> AmmoValuesChanged;
        // params: new dash cooldown value
        public event Action<int> DashCooldownValueChanged;
        // params: new quake cooldown value
        public event Action<int> QuakeCooldownValueChanged;

        [Header("Gameplay Parameters")]
        public float MaxHP;
        public float CurrentHP
        {
            get => currentHpValue;
            set
            {
                currentHpValue = value;
                HealthValueChanged?.Invoke(currentHpValue);
            }
        }

        public Camera Camera => this.camera;
        public bool IsFrozen { get; set; }


        [Header("Movement Parameters")]
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float crouchHeight = 0.5f;
        [SerializeField] private float gravityForce = -15f;

        [Header("Ability Parameters")]
        [SerializeField] private float dashDistance;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float slideDistance;
        [SerializeField] private float slideSpeed;
        [SerializeField] private float quakeFreezeTime;
        [SerializeField] private float quakeForce;
        [SerializeField] private float dashCooldown = 1.5f;
        [SerializeField] private float quakeCooldown = 1.5f;
        [SerializeField] private float quakeRadius = 2;
        [SerializeField] private float quakeDamage = 2;

        public bool IsDoubleJumpEnabled;
        public bool IsDashEnabled;
        public bool IsQuakeEnabled;

        #region Unlocks properties

        public float DashCooldown
        {
            get => dashCooldown;
            set => dashCooldown = value;
        }

        public float DashSpeed
        {
            get => dashSpeed;
            set => dashSpeed = value;
        }

        public float QuakeCooldown
        {
            get => quakeCooldown;
            set => quakeCooldown = value;
        }

        public float QuakeRadius
        {
            get => quakeRadius;
            set => quakeRadius = value;
        }

        public float QuakeDamage
        {
            get => quakeDamage;
            set => quakeDamage = value;
        }

        #endregion


        [Header("Shooting Parameters")]
        [SerializeField] private float scatterConeAngle = 3f;
        [SerializeField] private float roundsPerMinute = 300f;
        [SerializeField] private float reloadTime = 1.5f;
        [SerializeField] private int shotsMax = 30;

        public WeaponType UnlockedWeapons = (WeaponType)31;
        public WeaponType SelectedWeapon = 0;
        public ShootingHandlerState SelectedWeaponState => shootingHandler.State;

        [Header("Other settings")]
        [SerializeField] private float isGroundedCheckDistance = 0.1f;
        [SerializeField] private LayerMask enemiesLayer;
        [SerializeField] private int crosshairLength = 50;
        [SerializeField] private int crosshairWidth = 2;
        [SerializeField] private int initialAmmoAmountDebug = 45;
        [SerializeField] private LayerMask checkForGroundMask;
        [SerializeField] private float initializeFreezeSeconds = 1;

        // Components references
        private CharacterController controller;
        private Camera camera;
        private Transform capsuleTransform;
        private CapsuleCollider capsuleCollider;

        // Private variables
        private float currentHpValue;
        private bool isGrounded;
        private float verticalRotation = 0f;
        private float verticalVelocity = 0f;

        private bool isCrouching;
        private bool isDashing;
        private bool isSliding;
        private bool isVerticalFreeze;
        private bool isQuaking;
        private int jumpsMade = 0;
        private bool isInitialized;
        
        private Vector3 lastMoveDirection;
        private float timeSinceLastQuake = 0;
        private float timeSinceLastDash = 0;        

        private ShootingHandlerBase shootingHandler;

        [SerializeField] private Transform shootingPoint;

        private Dictionary<WeaponType, ShootingHandlerBase> shootingHandlers = new Dictionary<WeaponType, ShootingHandlerBase>();

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            camera = GetComponentInChildren<Camera>();
            capsuleTransform = GetComponentInChildren<MeshRenderer>().gameObject.transform;
            capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Init(PlayerProgressionData playerProgressionData)
        {
            CurrentHP = MaxHP;

            IsDoubleJumpEnabled = playerProgressionData.isDoubleJumpPurchased;
            IsDashEnabled = playerProgressionData.isDashPurchased;
            IsQuakeEnabled = playerProgressionData.isQuakePurchased;
            UnlockedWeapons = playerProgressionData.weaponPurchased;
           
            ReloadShootingHandlers();
            SelectWeapon(WeaponType.Pistol);

            StartCoroutine(SetInitializedInSeconds(initializeFreezeSeconds));
        }

        private IEnumerator SetInitializedInSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized || IsFrozen) return;
            CheckForGround();
            HandleRotation();
            HandleMovement();
            HandleShooting();
            HandleActions();
            ApplyGravity();
        }

        private void CheckForGround()
        {
            isGrounded = IsGrounded(isGroundedCheckDistance);

            if (isGrounded)
            {
                jumpsMade = 0;
                verticalVelocity = 0f;
                if (isQuaking)
                {
                    Quake();
                }
            }
        }

        private void HandleRotation()
        {
            // Rotate the player horizontally based on mouse movement
            float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0f, horizontalRotation, 0f);

            // Rotate the camera vertically based on mouse movement
            verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        private void HandleMovement()
        {
            if (!isDashing && !isQuaking && !isSliding)
            {
                // Move the player based on WASD input
                float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
                float sidewaysSpeed = Input.GetAxis("Horizontal") * movementSpeed;

                // SPRINT
                if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
                {
                    forwardSpeed *= sprintSpeed / movementSpeed;
                    sidewaysSpeed *= sprintSpeed / movementSpeed;
                }

                // CROUCH
                if (Input.GetKey(KeyCode.C))
                {
                    if (!isCrouching)
                    {
                        StartCrouch();
                    }

                    // On Crouch logic
                    forwardSpeed *= crouchSpeed / movementSpeed;
                    sidewaysSpeed *= crouchSpeed / movementSpeed;
                }
                else if (isCrouching)
                {
                    StopCrouch();
                }

                Vector3 speed = new Vector3(sidewaysSpeed, 0f, forwardSpeed);
                speed = transform.rotation * speed;

                controller.Move(speed * Time.deltaTime);
                lastMoveDirection = speed.normalized;
            }
        }

        private void HandleActions()
        {
            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift) && !isSliding)
            {
                StartSlide();
                StartCoroutine(C_Slide(lastMoveDirection));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            timeSinceLastDash += Time.deltaTime;
            if (IsDashEnabled && Input.GetKeyDown(KeyCode.E) && !isDashing && timeSinceLastDash > dashCooldown)
            {
                StartCoroutine(C_Dash(lastMoveDirection));
            }

            timeSinceLastQuake += Time.deltaTime;
            if (IsQuakeEnabled &&  Input.GetKeyDown(KeyCode.Q) && !isQuaking && timeSinceLastQuake > quakeCooldown)
            {
                StartCoroutine(C_Quake());
            }

            HandleWeaponSelection();
        }

        private void HandleWeaponSelection()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectWeapon(WeaponType.Pistol);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectWeapon(WeaponType.Rifle);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectWeapon(WeaponType.Shotgun);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectWeapon(WeaponType.BFG);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectWeapon(WeaponType.Railgun);
            }
        }

        private void SelectWeapon(WeaponType weapon)
        {
            if (UnlockedWeapons.HasFlag(weapon) && SelectedWeapon != weapon)
            {
                SelectedWeapon = weapon;

                if (shootingHandler != null)
                {
                    shootingHandler.AmmoStateChanged -= TriggerAmmoValuesChanged;
                }

                shootingHandler = shootingHandlers[weapon];
                shootingHandler.AmmoStateChanged += TriggerAmmoValuesChanged;

                WeaponSelected?.Invoke(weapon, shootingHandler.State);
            }
        }

        private void ReloadShootingHandlers()
        {
            foreach (var weaponType in Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>())
            {
                ShootingHandlerBase handler = null;
                ShootingHandlerArgs shootingHandlerArgs = GameManager.CurrentWeaponConfiguration.GetShootingHandlerArgsForWeapon(weaponType);
                shootingHandlerArgs.playerController = this;
                shootingHandlerArgs.initialAmmoAmount = initialAmmoAmountDebug;

                switch (weaponType)
                {
                    case WeaponType.Pistol:
                        handler = new HitScanShootingHandler((HitScanShootingHandlerArgs)shootingHandlerArgs);
                        break;
                    case WeaponType.Rifle:
                        handler = new HitScanShootingHandler((HitScanShootingHandlerArgs)shootingHandlerArgs);
                        break;
                    case WeaponType.Shotgun:
                        handler = new FractionShootingHandler((FractionShootingHandlerArgs)shootingHandlerArgs);
                        break;
                    case WeaponType.BFG:
                        handler = new ProjectileShootingHandler((ProjectileShootingHandlerArgs)shootingHandlerArgs);
                        break;
                    case WeaponType.Railgun:
                        handler = new HitScanShootingHandler((HitScanShootingHandlerArgs)shootingHandlerArgs);
                        break;
                }

                shootingHandlers[weaponType] = handler;
            }
        }

        private void HandleShooting()
        {
            shootingHandler.Update();
        }

        private void ApplyGravity()
        {
            if (!isGrounded && !isVerticalFreeze)
            {
                verticalVelocity += gravityForce * Time.deltaTime;
            }
            var vector = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;
            controller.Move(vector);
        }

        private IEnumerator C_Dash(Vector3 dashDir)
        {
            isDashing = true;
            isVerticalFreeze = true;
            Vector3 targetPosition;

            Vector3 point1 = capsuleCollider.gameObject.transform.position + capsuleCollider.center + Vector3.up * capsuleCollider.height / 2f - Vector3.up * capsuleCollider.radius;
            Vector3 point2 = capsuleCollider.gameObject.transform.position + capsuleCollider.center - Vector3.up * capsuleCollider.height / 2f + Vector3.up * capsuleCollider.radius;

            if (Physics.CapsuleCast(point1, point2, capsuleCollider.radius, dashDir, out var hitInfo, dashDistance))
            {
                targetPosition = transform.position + dashDir * (hitInfo.distance - capsuleCollider.radius);
            }
            else
            {
                targetPosition = transform.position + dashDir * dashDistance;
            }

            targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            while (transform.position != targetPosition)
            {
                controller.Move(Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime) - transform.position);
                verticalVelocity = 0;
                yield return null;
            }

            isVerticalFreeze = false;
            isDashing = false;
            timeSinceLastDash = 0;
        }

        private IEnumerator C_Slide(Vector3 slideDir)
        {
            float distanceSlided = 0f;
            while (distanceSlided < slideDistance)
            {
                var distanceDelta = slideSpeed * Time.deltaTime;
                distanceSlided += distanceDelta;
                controller.Move(slideDir.normalized * distanceDelta);
                yield return null;
            }

            isSliding = false;
            StopSlide();
        }

        private IEnumerator C_Quake()
        {
            isQuaking = true;
            isVerticalFreeze = true;
            verticalVelocity = 0;

            yield return new WaitForSeconds(quakeFreezeTime);

            // Quake
            verticalVelocity = -quakeForce;
        }

        private void Quake()
        {
            var colliders = Physics.OverlapSphere(transform.position, quakeRadius, enemiesLayer);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<IShootingTarget>(out var shootingTarget))
                {
                    Vector3 hitPoint = collider.ClosestPoint(transform.position);
                    float distanceToObject = Vector3.Distance(hitPoint, transform.position);
                    // the damage is less the further the object from epicenter
                    float actualDamage = quakeDamage * (1 - distanceToObject / quakeRadius);
                    shootingTarget.OnHit(hitPoint, actualDamage, DamageType.Explosion);
                }
            }
            isQuaking = false;
            isVerticalFreeze = false;
        }

        private void Jump()
        {
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                jumpsMade++;
            }
            else if (IsDoubleJumpEnabled && jumpsMade == 1)
            {
                verticalVelocity = jumpForce;
                jumpsMade++;
            }
        }

        private void StartCrouch()
        {
            // moving camera
            camera.transform.localPosition -= new Vector3(0, 0.5f, 0);
            // adjusting controller's trigger height
            controller.center -= new Vector3(0, 0.5f, 0);
            controller.height = crouchHeight;
            // setting capsule mesh crouching height
            var scale = capsuleTransform.localScale;
            scale.y = 0.5f;
            capsuleTransform.localScale = scale;
            // setting capsule mesh position
            var pos = capsuleTransform.localPosition;
            pos.y = 0.5f;
            capsuleTransform.localPosition = pos;
            // setting actual colliders height
            capsuleCollider.height = 0.5f;

            isCrouching = true;
        }

        private void StopCrouch()
        {
            // moving camera
            camera.transform.localPosition += new Vector3(0, 0.5f, 0);
            // adjusting controller's trigger height
            controller.center += new Vector3(0, 0.5f, 0);
            controller.height = 2f;
            // setting capsule mesh crouching height
            var scale = capsuleTransform.localScale;
            scale.y = 1f;
            capsuleTransform.localScale = scale;
            // setting capsule mesh position
            var pos = capsuleTransform.localPosition;
            pos.y = 1;
            capsuleTransform.localPosition = pos;
            // setting actual colliders height
            capsuleCollider.height = 1f;

            isCrouching = false;
        }

        private void StartSlide()
        {
            // moving camera
            camera.transform.localPosition -= new Vector3(0, 0.5f, 0);
            // adjusting controller's trigger height
            controller.center -= new Vector3(0, 0.5f, 0);
            controller.height = crouchHeight;
            // setting capsule mesh crouching height
            var scale = capsuleTransform.localScale;
            scale.y = 0.5f;
            capsuleTransform.localScale = scale;
            // setting capsule mesh position
            var pos = capsuleTransform.localPosition;
            pos.y = 0.5f;
            capsuleTransform.localPosition = pos;
            // setting actual colliders height
            capsuleCollider.height = 0.5f;

            isSliding = true;
        }

        private void StopSlide()
        {
            // moving camera
            camera.transform.localPosition += new Vector3(0, 0.5f, 0);
            // adjusting controller's trigger height
            controller.center += new Vector3(0, 0.5f, 0);
            controller.height = 2f;
            // setting capsule mesh crouching height
            var scale = capsuleTransform.localScale;
            scale.y = 1f;
            capsuleTransform.localScale = scale;
            // setting capsule mesh position
            var pos = capsuleTransform.localPosition;
            pos.y = 1;
            capsuleTransform.localPosition = pos;
            // setting actual colliders height
            capsuleCollider.height = 1f;

            isSliding = false;
        }

        public bool IsGrounded(float groundDistance = 0.1f)
        {
            Vector3 center = transform.position;
            Ray ray = new Ray(center, Vector3.down);
            return Physics.Raycast(ray, groundDistance, checkForGroundMask);
        }

        public void ReceiveDamage(float damageAmount)
        {
            CurrentHP -= damageAmount;
            if (CurrentHP <= 0)
            {
                Die();
            }
        }

        public void Heal(float healAmount)
        {
            if (healAmount > MaxHP - CurrentHP)
            {
                CurrentHP = MaxHP;
            }
            else
            {
                CurrentHP += healAmount;
            }
        }

        private void TriggerAmmoValuesChanged()
        {
            AmmoValuesChanged?.Invoke(shootingHandler.State);
        }

        public void GetAmmo(int ammoAmount, WeaponType weaponType)
        {
            print($"PLAYER GOT {ammoAmount} FOR {weaponType}");
            shootingHandlers[weaponType].FillAmmo(ammoAmount);
        }

        public void Die()
        {
            PlayerDied?.Invoke();
        }

        public void UpgradeWeapon(WeaponUpgradeData.WeaponUpgradeLevel weaponUpgrade, WeaponType weapon)
        {
            if(UnlockedWeapons.HasFlag(weapon))
            {
                shootingHandlers[weapon].SetUpgradedParams(weaponUpgrade.newReloadTimeValue, weaponUpgrade.newMaxCapacityValue, weaponUpgrade.newDamageValue);
            }            
        }

        public Dictionary<WeaponType, ShootingHandlerState> GetWeaponsAmmoInfo()
        {
            var dict = new Dictionary<WeaponType, ShootingHandlerState>();
            foreach (var weaponType in Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>())
            {
                if(UnlockedWeapons.HasFlag(weaponType))
                {
                    dict.Add(weaponType, shootingHandlers[weaponType].State);
                }
            }
            return dict;
        }

        private void OnGUI()
        {
            string info = "";
            GUI.Label(new Rect(10, 10, 600, 300), info);
            DrawCrosshair();
        }

        private void DrawCrosshair()
        {
            int centerX = Screen.width / 2;
            int centerY = Screen.height / 2;
            GUI.DrawTexture(new Rect(centerX - (crosshairLength / 2), centerY - (crosshairWidth / 2), crosshairLength, crosshairWidth), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX - (crosshairWidth / 2), centerY - (crosshairLength / 2), crosshairWidth, crosshairLength), Texture2D.whiteTexture);
        }
    }
}

