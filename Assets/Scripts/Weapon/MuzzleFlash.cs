using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float duration = 0.08f;

    private Coroutine routine;

    private void Awake()
    {
        if (flashImage == null)
        {
            Debug.LogError("MuzzleFlash: flashImage is NOT assigned!", this);
            return;
        }

        flashImage.enabled = false;
    }

    public void Play()
    {
        if (flashImage == null)
        {
            Debug.LogError("MuzzleFlash: cannot Play(), flashImage is null!", this);
            return;
        }

        Debug.Log("MuzzleFlash: Play() called", this);

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Debug.Log("MuzzleFlash: ON", this);
        flashImage.enabled = true;

        yield return new WaitForSeconds(duration);

        flashImage.enabled = false;
        Debug.Log("MuzzleFlash: OFF", this);

        routine = null;
    }
}