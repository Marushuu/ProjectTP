using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public CinemachineCamera aimCamera;
    public CinemachineCamera freeLookCamera;
    public float sensitivity = 1f;

    // Inputs
    InputAction moveAction;
    InputAction aimAction;
    InputAction lookAction;
    InputAction freeLookAction;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        moveAction = InputSystem.actions.FindAction("Move");
        aimAction = InputSystem.actions.FindAction("Aim");
        lookAction = InputSystem.actions.FindAction("Look");
        freeLookAction = InputSystem.actions.FindAction("Free Look");
    }

    private void Update()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (aimAction.IsPressed())
        {
            aimCamera.Priority = 2;
        } else {
            aimCamera.Priority = 0;
        }

        if (freeLookAction.IsPressed())
        {
            if (freeLookCamera.Priority == 3)
            {
                freeLookCamera.Priority = 0;
            } else {
                freeLookCamera.Priority = 3;
            }
        }
        
        // Vector3 direction = cameraTransform.forward - new Vector3
    }

    private void Move()
    {
        // Vector2 moveInput = moveAction.ReadValue<Vector2>();
        // transform.position += new Vector3(moveInput.x, 0, moveInput.y) * 5 * Time.deltaTime;
    }
}
