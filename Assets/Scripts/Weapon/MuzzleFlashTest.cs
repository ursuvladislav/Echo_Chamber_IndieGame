using UnityEngine;

public class MuzzleFlashTest : MonoBehaviour
{
    [SerializeField] private MuzzleFlash muzzleFlash;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (muzzleFlash == null)
            {
                Debug.LogError("MuzzleFlashTest: ссылка muzzleFlash не назначена!", this);
                return;
            }

            muzzleFlash.Play();
        }
    }
}