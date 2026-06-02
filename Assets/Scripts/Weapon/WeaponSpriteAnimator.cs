using UnityEngine;
using UnityEngine.UI;

public class WeaponSpriteAnimator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Image targetImage;

    [Header("Animation")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 12f;
    [SerializeField] private bool loop = true;

    private int currentFrame;
    private float timer;
    private bool isPlaying = true;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (frames != null && frames.Length > 0 && targetImage != null)
            targetImage.sprite = frames[0];
    }

    private void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0 || targetImage == null)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / fps;

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Length - 1;
                    isPlaying = false;
                    break;
                }
            }

            targetImage.sprite = frames[currentFrame];
        }
    }

    public void Play(Sprite[] newFrames, float newFps, bool shouldLoop)
    {
        if (newFrames == null || newFrames.Length == 0)
            return;

        frames = newFrames;
        fps = newFps;
        loop = shouldLoop;

        currentFrame = 0;
        timer = 0f;
        isPlaying = true;
        targetImage.sprite = frames[0];
    }

    public void Stop()
    {
        isPlaying = false;
    }
}