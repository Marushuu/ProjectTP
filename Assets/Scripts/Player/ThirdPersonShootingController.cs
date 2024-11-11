using Cinemachine;
using UnityEngine;
using StarterAssets;

public class ThirdPersonShootingController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform BulletProjectile;
    [SerializeField] private Transform playerGun;

    private ThirdPersonController _thirdPersonController;
    private StarterAssetsInputs _starterAssetsInputs;

    private void Awake()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            Vector3 mouseWorldPosition = Vector3.zero; // This is the position of the mouse in the world

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f); // This is the center of the screen
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint); // This is the ray that goes from the camera to the center of the screen
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                mouseWorldPosition = raycastHit.point;
            }

            if (_starterAssetsInputs.aim)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                _thirdPersonController.SetSensitivity(aimSensitivity);
                _thirdPersonController.SetRotateOnMove(false);

                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }
            else
            {
                aimVirtualCamera.gameObject.SetActive(false);
                _thirdPersonController.SetSensitivity(normalSensitivity);
                _thirdPersonController.SetRotateOnMove(true);
            }

            if (_starterAssetsInputs.shoot)
            {
                Vector3 aimDirection = (mouseWorldPosition - playerGun.position).normalized;
                Transform bullet = Instantiate(BulletProjectile, playerGun.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                _starterAssetsInputs.shoot = false;
                bullet.GetComponent<Rigidbody>().AddForce(playerGun.forward * 10f, ForceMode.Impulse);
            }
        }
    }
}
