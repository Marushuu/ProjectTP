using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ShootingController : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float projectileSpeed = 20f;
    public float fireRate = 0.5f;
    public float recoilAmount = 1f;
    public float recoilSpeed = 5f;
    [Header("References")]
    public LayerMask aimMask;
    public Transform gunObject;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public CinemachineCamera freelookCamera;

    private float nextFireTime;
    private InputAction fireAction;
    private bool isFiring;
    private Vector3 currentRecoil;
    private Quaternion currentGunRotation;

    private void Start()
    {
        fireAction = InputSystem.actions.FindAction("Shoot");

        fireAction.performed += OnFirePerformed;
        fireAction.canceled += OnFireCancelled;
        fireAction.Enable();

        currentGunRotation = gunObject.localRotation;
    }

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
            ApplyRecoil();
        }

        RecoverRecoil();
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            isFiring = true;
        }
        else if (context.interaction is TapInteraction)
        {
            isFiring = false;
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
                ApplyRecoil();
            }
            RecoverRecoil();
        }
    }

    private void OnFireCancelled(InputAction.CallbackContext context)
    {
        isFiring = false;
    }

    private void Shoot()
    {
        if (freelookCamera.Priority < 15)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            Vector3 shootPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
            {
                shootPoint = (hit.point - firePoint.position).normalized;
            }
            else
            {
                shootPoint = ray.direction;
            }

            Debug.DrawLine(firePoint.position, shootPoint, Color.yellow);

            firePoint.forward = shootPoint;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * projectileSpeed, ForceMode.Impulse);
    }

    private void ApplyRecoil()
    {
        Vector3 recoilDirection = -firePoint.forward * recoilAmount;
        currentRecoil = Vector3.Lerp(currentRecoil, recoilDirection, Time.deltaTime * recoilSpeed);

        Quaternion targetRotation = currentGunRotation * Quaternion.Euler(-recoilAmount, 0, 0);
        gunObject.localRotation = Quaternion.Slerp(gunObject.localRotation, targetRotation, Time.deltaTime * recoilSpeed);
    }

    private void RecoverRecoil()
    {
        if (currentRecoil.magnitude > 0.01f)
        {
            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, Time.deltaTime * recoilSpeed);
        }
    }
}
