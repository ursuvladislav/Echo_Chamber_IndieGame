using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera playerCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float runSpeed = 7.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float airControl = 0.4f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedForce = -2f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float minLookX = -80f;
    [SerializeField] private float maxLookX = 80f;

    [Header("Head Bob")]
    [SerializeField] private float walkBobSpeed = 7f;
    [SerializeField] private float walkBobAmount = 0.03f;
    [SerializeField] private float runBobSpeed = 11f;
    [SerializeField] private float runBobAmount = 0.06f;
    [SerializeField] private float bobReturnSpeed = 8f;

    [Header("FOV")]
    [SerializeField] private float normalFov = 60f;
    [SerializeField] private float runFov = 68f;
    [SerializeField] private float fovSmoothSpeed = 8f;

    [Header("Recoil")]
    [SerializeField] private float recoilKickSpeed = 20f;
    [SerializeField] private float recoilRecoverySpeed = 10f;
    [SerializeField] private float recoilYawMultiplier = 1f;

    private CharacterController controller;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float cameraPitch;

    private Vector3 cameraInitialLocalPos;
    private float bobTimer;

    private bool isRunning;
    private bool isMoving;

    private Vector2 currentRecoilOffset;
    private Vector2 targetRecoilOffset;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main;
    }

    private void Start()
    {
        if (cameraTransform != null)
            cameraInitialLocalPos = cameraTransform.localPosition;

        if (playerCamera != null)
            playerCamera.fieldOfView = normalFov;

        LockCursor();
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleHeadBob();
        HandleFov();

        if (Input.GetKeyDown(KeyCode.Escape))
            UnlockCursor();

        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            LockCursor();
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = (transform.right * inputX + transform.forward * inputZ).normalized;

        isMoving = inputDirection.magnitude > 0.01f;
        isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetHorizontalVelocity = inputDirection * targetSpeed;

        float control = isGrounded ? 1f : airControl;

        horizontalVelocity = Vector3.Lerp(
            horizontalVelocity,
            targetHorizontalVelocity,
            acceleration * control * Time.deltaTime
        );

        if (isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundedForce;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMove = horizontalVelocity;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // recoil плавно затухает к нулю
        targetRecoilOffset = Vector2.Lerp(
            targetRecoilOffset,
            Vector2.zero,
            recoilRecoverySpeed * Time.deltaTime
        );

        // текущее смещение догоняет целевое
        currentRecoilOffset = Vector2.Lerp(
            currentRecoilOffset,
            targetRecoilOffset,
            recoilKickSpeed * Time.deltaTime
        );

        // обычный взгляд мышью
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookX, maxLookX);

        // горизонталь: мышь + временный recoil
        transform.Rotate(Vector3.up * (mouseX + currentRecoilOffset.y * recoilYawMultiplier));

        // вертикаль: мышь + временный recoil
        float finalPitch = Mathf.Clamp(
            cameraPitch - currentRecoilOffset.x,
            minLookX,
            maxLookX
        );

        cameraTransform.localRotation = Quaternion.Euler(finalPitch, 0f, 0f);
    }

    private void HandleHeadBob()
    {
        if (cameraTransform == null)
            return;

        bool groundedAndMoving = controller.isGrounded && isMoving;

        if (groundedAndMoving)
        {
            float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = isRunning ? runBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * bobSpeed;

            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;

            Vector3 targetPos = cameraInitialLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPos,
                bobReturnSpeed * Time.deltaTime
            );
        }
        else
        {
            bobTimer = 0f;

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraInitialLocalPos,
                bobReturnSpeed * Time.deltaTime
            );
        }
    }

    private void HandleFov()
    {
        if (playerCamera == null)
            return;

        float targetFov = isRunning && controller.isGrounded ? runFov : normalFov;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFov,
            fovSmoothSpeed * Time.deltaTime
        );
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AddRecoil(float pitchAmount, float yawAmount)
    {
        targetRecoilOffset += new Vector2(pitchAmount, yawAmount);
    }
}