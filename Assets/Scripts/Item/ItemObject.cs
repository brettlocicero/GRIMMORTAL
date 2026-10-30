using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] protected ItemSO item;

    [Header("Item References")]
    [SerializeField] Animator movementAnimator;

    CharacterController playerCC;
    protected PlayerController playerController;
    protected Transform cameraTransform;

    float movementVelocity = 0f;
    float currentMovement = 0f;
    
    Vector3 baseLocalPosition;
    Quaternion baseLocalRotation;

    protected Vector3 posOffset = Vector3.zero;
    protected Quaternion rotOffset = Quaternion.identity;

    public ItemSO ItemData => item;

    protected virtual void Start()
    {
        playerCC = GetComponentInParent<CharacterController>();
        playerController = GetComponentInParent<PlayerController>();
        cameraTransform = GetComponentInParent<Camera>().transform;

        baseLocalPosition = transform.localPosition;
        baseLocalRotation = transform.localRotation;

        CursorManager.LockCursor();
    }

    protected virtual void Update()
    {
        HandleMovementAnimation();
        HandleItemPositionAndRotation();
    }

    void HandleMovementAnimation()
    {
        // if (playerController.IsMovementLocked()) return;

        float targetMovement = InputManager.Actions.Player.Move.ReadValue<Vector2>().magnitude;

        if (!playerCC.isGrounded)
            targetMovement = 0f;

        currentMovement = Mathf.SmoothDamp(currentMovement, targetMovement, ref movementVelocity, 0.1f);
        movementAnimator.SetFloat("Movement", currentMovement);
    }

    void HandleItemPositionAndRotation()
    {
        transform.localPosition = baseLocalPosition + posOffset;
        transform.localRotation = baseLocalRotation * rotOffset;
    }
}