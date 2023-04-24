using Assets._Core.Scripts;
using Assets._Core.Scripts.Player.ShootingHandlers;
using System.Collections;
using UnityEngine;
using Assets._Core.Scripts.Player.ShootingParameters;
public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float movementSpeed = 5f;
    public float sprintSpeed = 7.5f;
    public float crouchSpeed = 2.5f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 8f;
    public float crouchHeight = 0.5f;
    public float gravityForce = -15f;

    [Header("Ability Parameters")]
    public float dashDistance;
    public float dashSpeed;
    public float slideDistance;
    public float slideSpeed;
    public float shutterFreezeTime;
    public float shutterForce;
    public bool IsDoubleJumpEnabled;
    public float dashCooldown = 1.5f;
    public float shutterCooldown = 1.5f;

    [Header("Shooting Parameters")]
    public float scatterConeAngle = 3f;
    public float roundsPerMinute = 300f;
    public float reloadTime = 1.5f; 
    public int shotsMax = 30;
    public bool isProjectile;
    public ProjectileBase projectilePrefab;
    public BulletTrace bulletTracePrefab;

    [Header("Other settings")]
    [SerializeField] private float isGroundedCheckDistance = 0.1f;
    [SerializeField] private int crosshairLength = 50;
    [SerializeField] private int crosshairWidth = 2;

    // Components references
    private CharacterController controller;
    private Camera camera;
    private Transform capsuleTransform;
    private CapsuleCollider capsuleCollider;

    // Private variables
    private bool isGrounded;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    
    private bool isCrouching;
    private bool isDashing;
    private bool isSliding;

    private int jumpsMade = 0;

    private Vector3 lastMoveDirection;
    private bool isVerticalFreeze;
    private bool isShuttering;

    private float timeSinceLastShutter = 0;
    private float timeSinceLastDash = 0;

    private ShootingHandlerBase currentShootingHandler;

    [SerializeField] private Transform shootingPoint;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();
        capsuleTransform = GetComponentInChildren<MeshRenderer>().gameObject.transform;
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        currentShootingHandler = new HitScanShootingHandler(
            new HitScanShootingParameters
            {
                roundsPerMinute = this.roundsPerMinute, 
                reloadTime = this.reloadTime,
                gunShopCapacity = this.shotsMax,
                playerController = this,
                camera = this.camera,
                bulletTracePrefab = this.bulletTracePrefab
            });
    }

    private void Update()
    {
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
        if (!isDashing && !isShuttering && !isSliding)
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
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && timeSinceLastDash > dashCooldown)
        {
            StartCoroutine(C_Dash(lastMoveDirection));
        }

        timeSinceLastShutter += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q) && !isShuttering && timeSinceLastShutter > shutterCooldown)
        {
            StartCoroutine(C_Shutter());
        }
    }

    private void HandleShooting()
    {
        currentShootingHandler.Update();
    }

    private void ApplyGravity()
    {
        if (!isGrounded && !isVerticalFreeze)
        {
            verticalVelocity += gravityForce * Time.deltaTime;
        }
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    private IEnumerator C_Dash(Vector3 dashDir)
    {
        isDashing = true;
        isVerticalFreeze = true;
        Vector3 targetPosition;

        Vector3 point1 = capsuleCollider.gameObject.transform.position + capsuleCollider.center + Vector3.up * capsuleCollider.height / 2f - Vector3.up * capsuleCollider.radius;
        Vector3 point2 = capsuleCollider.gameObject.transform.position + capsuleCollider.center - Vector3.up * capsuleCollider.height / 2f + Vector3.up * capsuleCollider.radius;
        
        if(Physics.CapsuleCast(point1, point2, capsuleCollider.radius, dashDir, out var hitInfo, dashDistance))
        {
            targetPosition = transform.position + dashDir * (hitInfo.distance - capsuleCollider.radius);
        }
        else
        {
            targetPosition = transform.position + dashDir * dashDistance;
        }

        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        while(transform.position != targetPosition)
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

    private IEnumerator C_Shutter()
    {
        isShuttering = true;
        isVerticalFreeze = true;
        verticalVelocity = 0;

        yield return new WaitForSeconds(shutterFreezeTime);
        // Shutter
        verticalVelocity = -shutterForce;
                
        timeSinceLastShutter = 0;
        isShuttering = false;
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
        Vector3 center = transform.position + capsuleCollider.center;
        Ray ray = new Ray(center, Vector3.down);
        return Physics.Raycast(ray, groundDistance);
    }

    public void ReceiveDamage(float damage)
    {
        // implement health and damage
        print($"Damage taken: {damage}");
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 600, 300), $"IsDashing: {isDashing}. IsVerticalFrozen: {isVerticalFreeze}. IsSliding {isSliding}");
        
        int centerX = Screen.width / 2;
        int centerY = Screen.height / 2;

        GUI.DrawTexture(new Rect(centerX - (crosshairLength / 2), centerY - (crosshairWidth / 2), crosshairLength, crosshairWidth), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - (crosshairWidth / 2), centerY - (crosshairLength / 2), crosshairWidth, crosshairLength), Texture2D.whiteTexture);
    }
}