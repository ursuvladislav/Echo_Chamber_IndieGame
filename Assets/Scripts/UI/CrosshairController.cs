using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Current Type")]
    [SerializeField] private CrosshairType currentType = CrosshairType.None;

    [Header("Root Objects")]
    [SerializeField] private GameObject pistolRoot;
    [SerializeField] private GameObject shotgunRoot;
    [SerializeField] private GameObject rs12Root;

    [Header("Pistol Parts")]
    [SerializeField] private RectTransform pistolTop;
    [SerializeField] private RectTransform pistolBottom;
    [SerializeField] private RectTransform pistolLeft;
    [SerializeField] private RectTransform pistolRight;
    [SerializeField] private Image pistolCenterDot;

    [Header("Shotgun Parts")]
    [SerializeField] private RectTransform shotgunLeftArc;
    [SerializeField] private RectTransform shotgunRightArc;

    [Header("Shotgun Spread")]
    [SerializeField] private float shotgunIdleSpread = 14f;
    [SerializeField] private float shotgunMoveSpread = 22;
    [SerializeField] private float shotgunRunSpread = 34f;

    [Header("RS12 Parts")]
    [SerializeField] private RectTransform rs12TopLeft;
    [SerializeField] private RectTransform rs12TopRight;
    [SerializeField] private RectTransform rs12BottomLeft;
    [SerializeField] private RectTransform rs12BottomRight;
    [SerializeField] private Image rs12CenterDot;

    [Header("RS12 Limits")]
    [SerializeField] private float rs12MinSpread = 4f;
    [SerializeField] private float rs12DotGap = 1f;

    [Header("RS12 Spread")]
    [SerializeField] private float rs12IdleSpread = 6f;
    [SerializeField] private float rs12MoveSpread = 25f;
    [SerializeField] private float rs12RunSpread = 40f;

    [Header("Spread Settings")]
    [SerializeField] private float currentSpread = 6f;
    [SerializeField] private float targetSpread = 6f;

    [Header("Spread Presets")]
    [SerializeField] private float idleSpread = 6f;
    [SerializeField] private float moveSpread = 14f;
    [SerializeField] private float runSpread = 24f;

    [Header("Spread Smooth")]
    [SerializeField] private float spreadExpandSpeed = 14f;
    [SerializeField] private float spreadShrinkSpeed = 0.5f;

    [Header("Shot Spread Kick")]
    [SerializeField] private float shotSpreadRecoverySpeed = 12f;

    private float currentShotSpread;
    private float targetShotSpread;

    [Header("Center Dot")]
    [SerializeField] private float dotAlphaLerpSpeed = 10f;

    [Header("Test Input")]
    [SerializeField] private bool enableKeyboardTestSwitch = true;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRunning;


    [SerializeField] private float maxShotSpread = 20f;

    private void Start()
    {
        ApplyCrosshairState();
        ForceRefreshCurrentCrosshair();
    }

    private void Update()
    {
        HandleTestTypeSwitch();
        HandleTestMovementState();
        UpdateSpread();
        UpdateCurrentCrosshairVisual();
    }

    private void HandleTestTypeSwitch()
    {
        if (!enableKeyboardTestSwitch)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SetCrosshairType(CrosshairType.None);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCrosshairType(CrosshairType.Pistol);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCrosshairType(CrosshairType.Shotgun);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCrosshairType(CrosshairType.Uzi);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetCrosshairType(CrosshairType.RS12);
    }

    private void HandleTestMovementState()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        isMoving = Mathf.Abs(inputX) > 0.01f || Mathf.Abs(inputZ) > 0.01f;
        isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        targetSpread = GetTargetSpreadForCurrentCrosshair();
    }

    private void UpdateSpread()
    {
        float speed = targetSpread > currentSpread ? spreadExpandSpeed : spreadShrinkSpeed;

        currentSpread = Mathf.Lerp(currentSpread, targetSpread, speed * Time.deltaTime);

        targetShotSpread = Mathf.Lerp(targetShotSpread, 0f, shotSpreadRecoverySpeed * Time.deltaTime);
        currentShotSpread = Mathf.Lerp(currentShotSpread, targetShotSpread, shotSpreadRecoverySpeed * Time.deltaTime);
    }

    private void UpdateCurrentCrosshairVisual()
    {
        switch (currentType)
        {
            case CrosshairType.None:
                break;

            case CrosshairType.Pistol:
                UpdatePistolCrosshair();
                break;

            case CrosshairType.Shotgun:
                UpdateShotgunCrosshair();
                break;

            case CrosshairType.Uzi:
                UpdateUziCrosshair();
                break;

            case CrosshairType.RS12:
                UpdateRS12Crosshair();
                break;
        }
    }

    public void SetCrosshairType(CrosshairType newType)
    {
        if (currentType == newType)
            return;

        currentType = newType;
        ApplyCrosshairState();
        ForceRefreshCurrentCrosshair();
    }

    private void ApplyCrosshairState()
    {
        if (pistolRoot != null) pistolRoot.SetActive(currentType == CrosshairType.Pistol || currentType == CrosshairType.Uzi);
        if (shotgunRoot != null) shotgunRoot.SetActive(currentType == CrosshairType.Shotgun);
        if (rs12Root != null) rs12Root.SetActive(currentType == CrosshairType.RS12); 
    }

    private void ForceRefreshCurrentCrosshair()
    {
        UpdateCurrentCrosshairVisual();
    }

    private void UpdatePistolCrosshair()
    {
        float lineHalfHeight = pistolTop.sizeDelta.y * 0.5f;
        float lineHalfWidth = pistolLeft.sizeDelta.x * 0.5f;

        float dotHalfSize = pistolCenterDot.rectTransform.sizeDelta.x * 0.5f;

        float minVertical = lineHalfHeight + dotHalfSize + 2f;
        float minHorizontal = lineHalfWidth + dotHalfSize + 2f;

        float finalSpread = currentSpread + currentShotSpread;

        float spreadY = Mathf.Max(finalSpread, minVertical);
        float spreadX = Mathf.Max(finalSpread, minHorizontal);

        SetAnchoredPositionY(pistolTop, spreadY);
        SetAnchoredPositionY(pistolBottom, -spreadY);
        SetAnchoredPositionX(pistolLeft, -spreadX);
        SetAnchoredPositionX(pistolRight, spreadX);

        float targetDotAlpha = isMoving ? 0f : 1f;
        UpdateDotAlpha(pistolCenterDot, targetDotAlpha);
    }

    private void UpdateShotgunCrosshair()
    {
    float finalSpread = currentSpread + currentShotSpread;
    SetAnchoredPositionX(shotgunLeftArc, -finalSpread);
    SetAnchoredPositionX(shotgunRightArc, finalSpread);
    }

    private float GetTargetSpreadForCurrentCrosshair()
    {
        switch (currentType)
        {
            case CrosshairType.Shotgun:
                if (!isMoving) return shotgunIdleSpread;
                if (isRunning) return shotgunRunSpread;
                return shotgunMoveSpread;

            case CrosshairType.RS12:
                if (!isMoving) return rs12IdleSpread;
                if (isRunning) return rs12RunSpread;
                return rs12MoveSpread;

            case CrosshairType.Pistol:
            case CrosshairType.Uzi:
            default:
                if (!isMoving) return idleSpread;
                if (isRunning) return runSpread;
                return moveSpread;
        }
    }

    private void UpdateUziCrosshair()
    {
        float uziSpread = (currentSpread + currentShotSpread) * 1.2f;

        SetAnchoredPositionY(pistolTop, uziSpread);
        SetAnchoredPositionY(pistolBottom, -uziSpread);
        SetAnchoredPositionX(pistolLeft, -uziSpread);
        SetAnchoredPositionX(pistolRight, uziSpread);

        if (pistolCenterDot != null)
        {
            Color color = pistolCenterDot.color;
            color.a = 0f;
            pistolCenterDot.color = color;
        }
    }

    private void UpdateRS12Crosshair()
    {
        float finalSpread = currentSpread + currentShotSpread;

        SetAnchoredPosition(rs12TopLeft, -finalSpread, finalSpread);
        SetAnchoredPosition(rs12TopRight, finalSpread, finalSpread);
        SetAnchoredPosition(rs12BottomLeft, -finalSpread, -finalSpread);
        SetAnchoredPosition(rs12BottomRight, finalSpread, -finalSpread);

        float targetDotAlpha = isMoving ? 0f : 1f;
        UpdateDotAlpha(rs12CenterDot, targetDotAlpha);
    }

    private void UpdateDotAlpha(Image dotImage, float targetAlpha)
    {
        if (dotImage == null)
            return;

        Color color = dotImage.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, dotAlphaLerpSpeed * Time.deltaTime);
        dotImage.color = color;
    }

    private void SetAnchoredPositionX(RectTransform rect, float x)
    {
        if (rect == null)
            return;

        Vector2 pos = rect.anchoredPosition;
        pos.x = x;
        rect.anchoredPosition = pos;
    }

    private void SetAnchoredPositionY(RectTransform rect, float y)
    {
        if (rect == null)
            return;

        Vector2 pos = rect.anchoredPosition;
        pos.y = y;
        rect.anchoredPosition = pos;
    }

    private void SetAnchoredPosition(RectTransform rect, float x, float y)
    {
        if (rect == null)
            return;

        rect.anchoredPosition = new Vector2(x, y);
    }

    public void AddShotSpread(float amount)
    {
        targetShotSpread = Mathf.Min(targetShotSpread + amount, maxShotSpread);
    }
}