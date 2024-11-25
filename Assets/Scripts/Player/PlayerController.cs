using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;
    public CinemachineCamera freelookCamera;
    public AudioSource footstepAudio;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleJump();
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Convert input to world space based on camera orientation
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * moveDirection;
        moveDirection.Normalize();

        float speed = sprintAction.IsPressed() ? sprintSpeed : moveSpeed;
        characterController.Move(moveDirection * speed * Time.deltaTime);

        // Rotate character to face movement direction
        if (freelookCamera.Priority >= 15)
        {
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

        }

        // Play footstep audio
        if (isGrounded && characterController.velocity.magnitude > 0.1f && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
    }

    private void HandleGravity()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small negative to keep grounded
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
