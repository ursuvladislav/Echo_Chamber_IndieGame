using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CrosshairController crosshairController;

    [Header("Weapon Type")]
    [SerializeField] private CrosshairType weaponCrosshairType = CrosshairType.Pistol;

    [Header("Shooting")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 3f; // выстрелов в секунду
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Magazine")]
    [SerializeField] private int magazineSize = 8;
    [SerializeField] private float reloadTime = 1.2f;
    [SerializeField] private bool autoReloadWhenEmpty = true;

    [Header("Recoil")]
    [SerializeField] private float recoilPitchMin = 0.5f;
    [SerializeField] private float recoilPitchMax = 1.0f;
    [SerializeField] private float recoilYawMin = 0.03f;
    [SerializeField] private float recoilYawMax = 0.08f;

    [Header("Crosshair Kick")]
    [SerializeField] private float shotSpreadKick = 4f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRay = true;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main;

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        currentAmmo = magazineSize;
    }

    private void Start()
    {
        if (crosshairController != null)
        {
            crosshairController.SetCrosshairType(weaponCrosshairType);
        }
    }

    private void Update()
    {
        HandleReloadInput();
        HandleShootInput();
    }

    private void HandleShootInput()
    {
        if (isReloading)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            if (autoReloadWhenEmpty)
            {
                StartReload();
            }
            return;
        }

        Fire();
    }

    private void HandleReloadInput()
    {
        if (isReloading)
            return;

        if (!Input.GetKeyDown(KeyCode.R))
            return;

        if (currentAmmo >= magazineSize)
            return;

        StartReload();
    }

    private void Fire()
    {
        nextFireTime = Time.time + (1f / fireRate);
        currentAmmo--;

        ApplyRecoil();
        PerformRaycastShot();

        Debug.Log($"Pistol shot | Ammo: {currentAmmo}/{magazineSize}");

        if (currentAmmo <= 0 && autoReloadWhenEmpty)
        {
            StartReload();
        }

        if (crosshairController != null)
        {
            crosshairController.AddShotSpread(shotSpreadKick);
        }
    }

    private void PerformRaycastShot()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (drawDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            Debug.Log("Hit: " + hit.collider.name);

            DamageableTarget damageable = hit.collider.GetComponent<DamageableTarget>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private float GetRandomHorizontalRecoil()
    {
        float amount = Random.Range(recoilYawMin, recoilYawMax);
        return Random.value > 0.5f ? amount : -amount;
    }

    private void ApplyRecoil()
    {
        if (playerController == null)
            return;

        float pitch = Random.Range(recoilPitchMin, recoilPitchMax);
        float yaw = GetRandomHorizontalRecoil();

        playerController.AddRecoil(pitch, yaw);
    }

    private void StartReload()
    {
        if (!gameObject.activeInHierarchy)
            return;

        StartCoroutine(ReloadRoutine());
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload complete");
    }
}