using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float shutterFreezeTime;
    public float shutterForce;
    public bool IsDoubleJumpEnabled;

    // Components references
    private CharacterController controller;
    private Camera camera;
    private Transform capsuleTransform;
    private CapsuleCollider capsuleCollider;

    // Private variables
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private bool isCrouching = false;
    private bool isDashing;
    private bool isGrounded;
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
    }

    void Update()
    {
        isGrounded = IsGrounded();
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

        // SHIFTING
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            forwardSpeed *= sprintSpeed / movementSpeed;
            sidewaysSpeed *= sprintSpeed / movementSpeed;
        }

        // CROUCH
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if(!isCrouching)
            {
                StartCrouch();
            }

            // on crouch logic
            forwardSpeed *= crouchSpeed / movementSpeed;
            sidewaysSpeed *= crouchSpeed / movementSpeed;
            //
        }
        else if(isCrouching)
        {
            StopCrouch(); 
        }

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("enter");
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.F) && !isDashing)
        {
            StartCoroutine(C_Dash(lastMoveDirection));
        }

        if (Input.GetKeyDown(KeyCode.G) && !isShuttering)
        {
            StartCoroutine(C_Shutter());
        }

        if (!isDashing && !isShuttering)
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



    private IEnumerator C_Dash(Vector3 dashDir)
    {
        isDashing = true;
        isVerticalFreeze = true;
        Vector3 targetPosition;
        
        Ray ray = new Ray(transform.position + new Vector3(0, capsuleCollider.height / 2, 0), dashDir);
        if(Physics.Raycast(ray, out var hitInfo, dashDistance))
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

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"IsDasing: {isDashing}, {isVerticalFreeze}");
    }
}