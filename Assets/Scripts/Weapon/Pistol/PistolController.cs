using UnityEngine;

public class PistolController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private WeaponSpriteAnimator weaponAnimator;
    [SerializeField] private MuzzleFlash muzzleFlash;

    [Header("Animations")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] shootFrames;

    [Header("FPS")]
    [SerializeField] private float idleFps = 12f;
    [SerializeField] private float shootFps = 24f;

    private bool isShooting;

    private void Start()
    {
        PlayIdle();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (isShooting)
            return;

        if (shootFrames == null || shootFrames.Length == 0)
        {
            Debug.LogWarning("Shoot frames are not assigned.", this);

            if (muzzleFlash != null)
                muzzleFlash.Play();

            return;
        }

        isShooting = true;

        weaponAnimator.Play(shootFrames, shootFps, false);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        float shootDuration = shootFrames.Length / shootFps;
        Invoke(nameof(PlayIdle), shootDuration);
    }

    public void PlayIdle()
    {
        isShooting = false;

        if (idleFrames != null && idleFrames.Length > 0)
            weaponAnimator.Play(idleFrames, idleFps, true);
    }
}