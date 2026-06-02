using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMuzzleFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.08f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (flashImage != null)
            flashImage.enabled = false;
    }

    public void PlayFlash()
    {
        if (flashImage == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        flashImage.enabled = false;
        currentRoutine = null;
    }
}