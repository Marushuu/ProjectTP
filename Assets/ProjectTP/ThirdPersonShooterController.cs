using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using System.Numerics;
using Unity.VisualScripting;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform aimTransform;

    private ThirdPersonController _thirdPersonController;
    private StarterAssetsInputs _starterAssetsInputs;

    private void Awake()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        UnityEngine.Vector3 mouseWorldPosition = UnityEngine.Vector3.zero; // This is the position of the mouse in the world

        UnityEngine.Vector2 screenCenterPoint = new UnityEngine.Vector2(Screen.width / 2f, Screen.height / 2f); // This is the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint); // This is the ray that goes from the camera to the center of the screen
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            aimTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        if (_starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            _thirdPersonController.SetSensitivity(aimSensitivity);
            _thirdPersonController.SetRotateOnMove(false);

            UnityEngine.Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            UnityEngine.Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = UnityEngine.Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            _thirdPersonController.SetSensitivity(normalSensitivity);
            _thirdPersonController.SetRotateOnMove(true);
        }

        if (_starterAssetsInputs.shoot)
        {
            if (hitTransform != null)
            {
                if (hitTransform.GetComponent<Target>() != null)
                {
                    hitTransform.GetComponent<Target>().TakeDamage(1);
                }
                else
                {
                    Debug.Log("Hit something that is not a target");
                }
            }
            _starterAssetsInputs.shoot = false;
        }
    }
}
