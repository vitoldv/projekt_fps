using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float movementSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 5f;
    public float crouchHeight = 0.5f;
    public float gravityForce = -9.81f;

    [Header("Ability Parameters")]
    public float dashDistance;
    public float dashSpeed;
    public float slideDistance;
    public float slideSpeed;
    public float shutterFreezeTime;
    public float shutterForce;
    public bool IsDoubleJumpEnabled;

    [Header("Shooting Parameters")]
    public float scatterConeAngle = 3f;  // Angle of the scatter cone in degrees
    public float roundsPerMinute = 300f;  // Number of rounds fired per minute
    public float reloadTime = 1.5f;  // Time it takes to reload the weapon
    private float timeBetweenShots;  // Time between shots based on rounds per minute
    private bool isReloading;  // Flag to indicate if the weapon is currently reloading
    private float timeSinceLastShot;  // Time elapsed since the last shot
    private int shotsMade = 0;
    public int shotsMax = 30;
    public bool isProjectile;
    public ProjectileBase projectile;

    [SerializeField] private float isGroundedCheckDistance = 0.1f;

    // Components references
    private CharacterController controller;
    private Camera camera;
    private Transform capsuleTransform;
    private CapsuleCollider capsuleCollider;
    public BulletTrace bulletTrace;

    // Private variables
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private bool isCrouching = false;
    private bool isDashing;
    private bool isGrounded;
    private bool isSliding;
    private int jumpsMade;
    private Vector3 lastMoveDirection;
    private bool isVerticalFreeze;
    private bool isShuttering;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();
        capsuleTransform = GetComponentInChildren<MeshRenderer>().gameObject.transform;
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        timeBetweenShots = 60f / roundsPerMinute;
    }

    void Update()
    {
        isGrounded = IsGrounded(isGroundedCheckDistance);
        if (isGrounded)
        {
            jumpsMade = 0;
            verticalVelocity = 0f;
        }

        // Rotate the player horizontally based on mouse movement
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0f, horizontalRotation, 0f);

        // Rotate the camera vertically based on mouse movement
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Move the player based on WASD input
        float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        float sidewaysSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift) && !isSliding)
        {
            StartCrouch();
            isCrouching = false;
            isSliding = true;
            StartCoroutine(C_Slide(lastMoveDirection));
        }
        else if (!isSliding)
        {
            // SHIFTING
            if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
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

                // on crouch logic
                forwardSpeed *= crouchSpeed / movementSpeed;
                sidewaysSpeed *= crouchSpeed / movementSpeed;
                //
            }
            else if (isCrouching)
            {
                StopCrouch();
            }
        }


        // JUMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("enter");
            Jump();
        }

        if(Input.GetMouseButton(0) && !isReloading && timeSinceLastShot >= timeBetweenShots)
        {
            Shoot();
            timeSinceLastShot = 0f;
        }

        timeSinceLastShot += Time.deltaTime;

        if ((shotsMade == shotsMax || Input.GetKeyDown(KeyCode.R)) && !isReloading)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.F) && !isDashing)
        {
            StartCoroutine(C_Dash(lastMoveDirection));
        }

        if (Input.GetKeyDown(KeyCode.G) && !isShuttering)
        {
            StartCoroutine(C_Shutter());
        }

        if (!isDashing && !isShuttering && !isSliding)
        {
            Vector3 speed = new Vector3(sidewaysSpeed, 0f, forwardSpeed);
            speed = transform.rotation * speed;
            controller.Move(speed * Time.deltaTime);
            lastMoveDirection = speed.normalized;
        }
       

        // Apply gravity to the player
        if (!isGrounded && !isVerticalFreeze)
        {
            verticalVelocity += gravityForce * Time.deltaTime;
        }
        
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
        
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        isReloading = false;
        shotsMade = 0;
    }

    private IEnumerator C_Dash(Vector3 dashDir)
    {
        isDashing = true;
        isVerticalFreeze = true;
        Vector3 targetPosition;

        Vector3 point1 = capsuleCollider.gameObject.transform.position + capsuleCollider.center + Vector3.up * capsuleCollider.height / 2f - Vector3.up * capsuleCollider.radius;
        Vector3 point2 = capsuleCollider.gameObject.transform.position + capsuleCollider.center - Vector3.up * capsuleCollider.height / 2f + Vector3.up * capsuleCollider.radius;
        
        //Ray ray = new Ray(transform.position + new Vector3(0, capsuleCollider.height / 2, 0), dashDir);

        //if(Physics.Raycast(ray, out var hitInfo, dashDistance))
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
            Debug.DrawLine(transform.position, targetPosition, Color.red);
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            controller.Move(Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime) - transform.position);
            verticalVelocity = 0;
            yield return null;
        }

        isVerticalFreeze = false;
        isDashing = false;
    }

    private IEnumerator C_Slide(Vector3 slideDir)
    {
        //Ray ray = new Ray(transform.position + new Vector3(0, capsuleCollider.height / 2, 0), dashDir);
        //if (Physics.Raycast(ray, out var hitInfo, dashDistance))
        //{
        //    targetPosition = transform.position + dashDir * (hitInfo.distance - capsuleCollider.radius);
        //}
        //else
        //{
        //    targetPosition = transform.position + dashDir * dashDistance;
        //}

        float distanceSlided = 0f;
        while (distanceSlided < slideDistance)
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            var distanceDelta = slideSpeed * Time.deltaTime;
            distanceSlided += distanceDelta;
            controller.Move(slideDir.normalized * distanceDelta);
            yield return null;
        }

        isSliding = false;
        StopCrouch();
    }

    private IEnumerator C_Shutter()
    {
        verticalVelocity = 0;
        isVerticalFreeze = true;
        isShuttering = true;
        yield return new WaitForSeconds(shutterFreezeTime);
        verticalVelocity = -shutterForce;
        isVerticalFreeze = false;
        isShuttering = false;
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
        print("1");
        camera.transform.localPosition -= new Vector3(0, 0.5f, 0);

        controller.center -= new Vector3(0, 0.5f, 0);
        controller.height = crouchHeight;
        // setting capsule crouching height
        var scale = capsuleTransform.localScale;
        scale.y = 0.5f;
        capsuleTransform.localScale = scale;
          
        var pos = capsuleTransform.localPosition;
        pos.y = 0.5f;
        capsuleTransform.localPosition = pos;
        capsuleCollider.height = 0.5f;
        isCrouching = true;
    }

    private void StopCrouch()
    {
        print("2");
        camera.transform.localPosition += new Vector3(0, 0.5f, 0);

        isCrouching = false;
        controller.height = 2f;

        var scale = capsuleTransform.localScale;
        scale.y = 1f;
        capsuleTransform.localScale = scale;

        var pos = capsuleTransform.localPosition;
        pos.y = 1;
        capsuleTransform.localPosition = pos;

        capsuleCollider.height = 1f;
        controller.center += new Vector3(0, 0.5f, 0);
    }

    public bool IsGrounded(float groundDistance = 0.1f)
    {
        // Calculate the center of the game object
        Vector3 center = transform.position + capsuleCollider.center;

        // Create a raycast from the center of the game object downwards
        Ray ray = new Ray(center, Vector3.down);

        // Check if the ray hits any collider within the given distance
        return Physics.Raycast(ray, groundDistance);
    }

    private void Shoot()
    {
        shotsMade++;

        // Calculate the direction of the bullet based on the scatter cone angle
        Quaternion scatterConeRotation = Quaternion.Euler(Random.Range(-scatterConeAngle, scatterConeAngle),
                                                          Random.Range(-scatterConeAngle, scatterConeAngle),
                                                          0f);

        var shootPoint = camera.transform.position - new Vector3(0f, 0.2f, 0f);
        var shootDir = scatterConeRotation * camera.transform.forward;
        
        if(isProjectile)
        {
            var prj = Instantiate(projectile, camera.transform.position + camera.transform.forward * 2, Quaternion.identity);
            prj.Init(shootDir, 2);
        }
        else
        {
            RaycastHit hit;
            var ray = new Ray(camera.transform.position, shootDir);
            Vector3 bulletReachPoint;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                bulletReachPoint = hit.point;
                if (hit.collider.TryGetComponent<IShootingTarget>(out var target) && target != null)
                {
                    target.OnHit(hit.point);
                }
            }
            else
            {
                bulletReachPoint = default;
            }
            var trace = Instantiate(bulletTrace);
            trace.Init(shootPoint, shootDir, bulletReachPoint);
        }
    }

    public int lineLength = 50;
    public int lineWidth = 2;

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 600, 300), $"IsDashing: {isDashing}. IsVerticalFrozen: {isVerticalFreeze}. IsReloading: {isReloading}. IsSliding {isSliding}");
        
        int centerX = Screen.width / 2;
        int centerY = Screen.height / 2;

        GUI.DrawTexture(new Rect(centerX - (lineLength / 2), centerY - (lineWidth / 2), lineLength, lineWidth), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - (lineWidth / 2), centerY - (lineLength / 2), lineWidth, lineLength), Texture2D.whiteTexture);
    }
}