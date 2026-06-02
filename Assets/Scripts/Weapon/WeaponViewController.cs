using UnityEngine;

public class WeaponViewController : MonoBehaviour
{
    [SerializeField] private WeaponSpriteAnimator animator;

    [Header("Pistol")]
    [SerializeField] private Sprite[] pistolIdle;
    [SerializeField] private Sprite[] pistolWalk;
    [SerializeField] private Sprite[] pistolRun;
    [SerializeField] private Sprite[] pistolShoot;
    [SerializeField] private Sprite[] pistolReload;
    [SerializeField] private Sprite[] pistolLowHP;
    [SerializeField] private Sprite[] pistolMidHP;

    [SerializeField] private float fps = 12f;

    private void Start()
    {
        PlayIdle();
    }

    public void PlayIdle()
    {
        animator.Play(pistolIdle, fps, true);
    }

    public void PlayWalk()
    {
        animator.Play(pistolWalk, fps, true);
    }

    public void PlayRun()
    {
        animator.Play(pistolRun, fps, true);
    }

    public void PlayShoot()
    {
        animator.Play(pistolShoot, fps, false);
    }

    public void PlayReload()
    {
        animator.Play(pistolReload, fps, false);
    }

    public void PlayLowHP()
    {
        animator.Play(pistolLowHP, fps, true);
    }

    public void PlayMidHP()
    {
        animator.Play(pistolMidHP, fps, true);
    }
}