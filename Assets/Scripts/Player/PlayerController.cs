using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    [Header("Crouch Settings")]
    public float crouchHeight = 0.5f;
    public float standHeight = 1f;

    [Header("Gravity and Grounding")]
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    public float groundOffset = 0.4f;
    public LayerMask groundMask;

    [Header("Sound Settings")]
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds;
    public AudioClip[] crouchSounds;
    public AudioClip[] runningSounds;
    public float walkStepInterval = 0.5f;
    public float crouchStepInterval = 0.7f;
    public float sprintStepInterval = 0.3f;
    public float velocityThreshold = 2.0f;

    [Header("References")]
    public Transform cameraTransform;
    public CinemachineCamera freelookCamera;
    public Transform playerModel;
    public Transform weaponTransform;
    public Transform boltTransform;
    public LayerMask aimMask;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private float nextStepTime;
    private int lastPlayedIndex = -1;
    private bool isGrounded;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    public MovementState movementState;
    public enum MovementState
    {
        Idle,
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");
    }

    private void Update()
    {
        StateHandler();
        HandleCrouch();
        HandleMovement();
        HandleFootsteps();
        HandleGravity();
        HandleJump();
        if (freelookCamera.Priority < 15)
        {
            HandleWeaponAim();
        }
    }

    private void StateHandler()
    {
        if (isGrounded)
        {
            if (sprintAction.IsPressed())
            {
                movementState = MovementState.Sprinting;
            }
            else if (crouchAction.IsPressed())
            {
                movementState = MovementState.Crouching;
            }
            else
            {
                movementState = MovementState.Walking;
            }
        }
        else
        {
            movementState = MovementState.Air;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Convert input to world space based on camera orientation
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveDirection;
        moveDirection.Normalize();

        isSprinting = sprintAction.IsPressed();
        float speed = isSprinting ? sprintSpeed : moveSpeed;

        if (isCrouching)
        {
            characterController.Move(moveDirection * crouchSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }

        // Rotate character to face movement direction
        if (freelookCamera.Priority >= 15)
        {
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void HandleFootsteps()
    {
        float currentStepInterval = 0f;

        switch (movementState)
        {
            case MovementState.Walking:
                currentStepInterval = walkStepInterval;
                break;
            case MovementState.Sprinting:
                currentStepInterval = sprintStepInterval;
                break;
            case MovementState.Crouching:
                currentStepInterval = crouchStepInterval;
                break;
        }

        if (isGrounded && Time.time > nextStepTime && characterController.velocity.magnitude > velocityThreshold)
        {
            PlayFootStepSounds();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    private void PlayFootStepSounds()
    {
        int randomIndex;
        if (footstepSounds.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            randomIndex = Random.Range(0, footstepSounds.Length - 1);
            if (randomIndex >= lastPlayedIndex)
            {
                randomIndex++;
            }
        }

        lastPlayedIndex = randomIndex;
        footstepSource.clip = footstepSounds[randomIndex];
        footstepSource.Play();
    }

    private void HandleGravity()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, characterController.radius - 0.05f, groundMask, QueryTriggerInteraction.Ignore);

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (jumpAction.triggered && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleWeaponAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        Vector3 aimPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = ray.origin + ray.direction * 1000f;
        }

        Debug.DrawLine(boltTransform.position, aimPoint, Color.cyan);
        Debug.DrawLine(weaponTransform.position, aimPoint, Color.green);

        Vector3 aimDirection = (aimPoint - weaponTransform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

        Quaternion initialRotationOffset = Quaternion.Euler(0, 180, 0);

        targetRotation *= initialRotationOffset;

        weaponTransform.rotation = Quaternion.Slerp(weaponTransform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private void HandleCrouch()
    {
        if (crouchAction.IsPressed())
        {
            isCrouching = true;
            playerModel.localScale = new Vector3(1, crouchHeight, 1);
            characterController.height = crouchHeight;
        }
        else
        {
            isCrouching = false;
            playerModel.localScale = new Vector3(1, 1, 1);
            characterController.height = 2f;
        }
    }
}
