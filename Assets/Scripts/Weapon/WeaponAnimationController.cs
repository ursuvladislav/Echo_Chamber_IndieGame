using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayIdle()
    {
        animator.Play("pistol_idle");
    }

    public void PlayWalk()
    {
        animator.Play("pistol_walk");
    }

    public void PlayRun()
    {
        animator.Play("pistol_run");
    }

    public void PlayShoot()
    {
        animator.Play("pistol_shoot");
    }

    public void PlayReload()
    {
        animator.Play("pistol_reload");
    }

    public void PlayLowHP()
    {
        animator.Play("pistol_lowHP");
    }

    public void PlayMidHP()
    {
        animator.Play("pistol_midHP");
    }
}