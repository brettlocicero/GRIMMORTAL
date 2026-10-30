using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] CinemachineCamera cinemachineCam;
    [SerializeField] AudioSource audioSource;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float gravity = -30f;
    [SerializeField] float jumpHeight = 1.5f;

    [Header("Air")]
    [SerializeField] float airControl = 4f;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 28f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 0.6f;
    [SerializeField] AudioClip dashSound;

    [Header("Ground Stick")]
    [SerializeField] float groundStickVelocity = 8f;
    [SerializeField] float minAirborneTimeForLandSound = 0.15f;

    [Header("Look")]
    [SerializeField] float mouseSensitivity = 0.15f;

    [Header("Head Bob")]
    [SerializeField] float bobFrequency = 10f;
    [SerializeField] float bobAmount = 0.05f;
    [SerializeField] float bobSideAmount = 0.03f;
    [SerializeField] float bobSmoothSpeed = 8f;

    [Header("Head Tilt")]
    [SerializeField] float maxTilt = 5f;
    [SerializeField] float tiltSmoothSpeed = 8f;

    [Header("Audio")]
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;
    [SerializeField] float landSoundVelocityThreshold = -4f;
    [SerializeField] float footstepInterval = 0.45f;

    CharacterController controller;

    Vector2 moveInput;
    Vector2 lookInput;
    Vector3 horizontalVelocity;

    float verticalVelocity;
    float cameraPitch;

    float footstepTimer;
    float airborneTimer;

    bool wasGrounded;

    Vector3 cameraDefaultPosition;
    float bobTimer;
    float currentTilt;

    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;
    Vector3 dashDirection;

    public bool SensitivityLocked { get; set; }
    public bool MovementLocked { get; set; }

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        cameraDefaultPosition = cameraTransform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        dashCooldownTimer -= Time.deltaTime;

        ReadInput();

        HandleHeadBob();
        HandleHeadTilt();

        Look();
        Move();
    }

    void ReadInput()
    {
        moveInput = InputManager.Actions.Player.Move.ReadValue<Vector2>();
        lookInput = InputManager.Actions.Player.Look.ReadValue<Vector2>();

        if (MovementLocked) moveInput = Vector2.zero;
        if (SensitivityLocked) lookInput = Vector2.zero;

        bool dashPressed = InputManager.Actions.Player.Dash.WasPressedThisFrame();
        bool canDash = dashPressed && !isDashing && !MovementLocked && dashCooldownTimer <= 0f;
        if (canDash)
            StartDash();
    }

    void StartDash()
    {
        Vector3 input = transform.right * moveInput.x + transform.forward * moveInput.y;
        dashDirection = input.sqrMagnitude > 0.01f ? input.normalized : transform.forward;

        isDashing = true;
        dashTimer = 0f;
        verticalVelocity = 0f;

        if (dashSound)
            audioSource.PlayOneShot(dashSound);
    }

    void HandleDash()
    {
        dashTimer += Time.deltaTime;

        // Stays fast for most of the dash, then eases out
        float progress = dashTimer / dashDuration;
        float speed = Mathf.Lerp(dashSpeed, moveSpeed, progress * progress);

        controller.Move(dashDirection * speed * Time.deltaTime);

        if (progress >= 1f)
            EndDash();
    }

    void EndDash()
    {
        isDashing = false;
        dashCooldownTimer = dashCooldown;

        // Carry a little momentum if the dash ends in the air
        horizontalVelocity = dashDirection * moveSpeed;
    }

    void HandleHeadBob()
    {
        bool moving = moveInput.magnitude > 0.1f && controller.isGrounded;

        Vector3 targetPosition = cameraDefaultPosition;

        if (moving)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float bobY = Mathf.Sin(bobTimer) * bobAmount;
            float bobX = Mathf.Cos(bobTimer * 0.5f) * bobSideAmount;

            targetPosition += new Vector3(bobX, bobY, 0f);
        }

        else
        {
            bobTimer = 0f;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * bobSmoothSpeed);
    }

    void HandleHeadTilt()
    {
        float targetTilt = -moveInput.x * maxTilt;

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmoothSpeed);
        cinemachineCam.Lens.Dutch = currentTilt;
    }

    void Move()
    {
        if (isDashing)
        {
            HandleDash();
        }

        else
        {
            HandleWalk();
        }
    }

    void HandleWalk()
    {
        bool grounded = controller.isGrounded;
        float fallVelocity = verticalVelocity;

        if (grounded)
            verticalVelocity = -groundStickVelocity;

        Vector3 inputVelocity = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;

        // Grounded follows input directly, airborne keeps momentum and only drifts toward input
        horizontalVelocity = grounded
            ? inputVelocity
            : Vector3.MoveTowards(horizontalVelocity, inputVelocity, airControl * Time.deltaTime);

        HandleJump(grounded);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 fullMove = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(fullMove * Time.deltaTime);

        HandleGroundSounds(grounded, fallVelocity);

        wasGrounded = grounded;
    }

    void HandleJump(bool grounded)
    {
        bool jumpPressed = InputManager.Actions.Player.Jump.WasPressedThisFrame();

        if (grounded && !MovementLocked && jumpPressed)
            Jump();
    }

    void Jump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (jumpSound)
            audioSource.PlayOneShot(jumpSound);
    }

    void HandleGroundSounds(bool grounded, float fallVelocity)
    {
        if (!grounded)
            airborneTimer += Time.deltaTime;

        // Landing
        if (!wasGrounded && grounded)
        {
            if (landSound && fallVelocity < landSoundVelocityThreshold && airborneTimer >= minAirborneTimeForLandSound)
                audioSource.PlayOneShot(landSound);

            airborneTimer = 0f;
        }

        // Footsteps
        if (grounded && moveInput.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }

        else
        {
            footstepTimer = 0;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length > 0)
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;

        verticalVelocity = 0f;
    }
}