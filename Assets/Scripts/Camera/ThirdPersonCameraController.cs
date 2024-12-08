using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera thirdPersonCamera;
    public CinemachineCamera aimCamera;
    public CinemachineCamera freeLookCamera;

    [Header("Input Settings")]
    public Transform playerCameraObject;
    public Transform playerModel;
    public float mouseSensitivity = 25f;

    [Header("Rotation Limits")]
    public float pitchMin = -35f; // Minimum pitch angle
    public float pitchMax = 35f; // Maximum pitch angle

    // Third Person Follow
    private CinemachineThirdPersonFollow thirdPersonFollow;

    // Rotation values
    private float yaw; // Horizontal Axis
    private float pitch; // Vertical Axis
    private Vector2 lookInput;

    // Inputs
    private InputAction lookAction;
    private InputAction aimAction;
    private InputAction freeLookAction;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction = InputSystem.actions.FindAction("Look");
        aimAction = InputSystem.actions.FindAction("Aim");
        freeLookAction = InputSystem.actions.FindAction("Free Look");

        thirdPersonFollow = thirdPersonCamera.GetComponent<CinemachineThirdPersonFollow>();
    }

    private void Update()
    {
        CameraSwitching();
        if (freeLookCamera.Priority < 15)
        {
            RotateCamera();
        }
    }

    private void CameraSwitching()
    {
        // Switch to free look when holding C
        if (freeLookAction.IsPressed())
        {
            thirdPersonFollow.CameraSide = 0.5f;
            freeLookCamera.Priority = 15;
            thirdPersonCamera.Priority = 10;
            aimCamera.Priority = 0;
        }
        // Switch to aim camera when holding right-click
        else if (aimAction.IsPressed())
        {
            thirdPersonFollow.CameraSide = 1f;
            aimCamera.Priority = 15;
            thirdPersonCamera.Priority = 10;
            freeLookCamera.Priority = 0;
        }
        // Default to third person camera
        else
        {
            thirdPersonFollow.CameraSide = 1f;
            thirdPersonCamera.Priority = 15;
            freeLookCamera.Priority = 0;
            aimCamera.Priority = 0;
        }
    }

    private void RotateCamera()
    {
        // Read input values
        lookInput = lookAction.ReadValue<Vector2>();

        // Update yaw and pitch values
        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;

        // Clamp pitch value
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Rotate camera along the x and y-axis
        playerCameraObject.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Rotate player model along the x-axis
        playerModel.rotation = Quaternion.Euler(0, yaw, 0);
    }
}
