using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyEnemyMovement : EnemyMovement
{
    enum MoveMode { Idle, Seeking, Fleeing }

    [Header("Movement")]
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float stoppingDistance = 0.1f;
    [SerializeField] bool ignoreMovementRotation = false;

    [Header("Separation")]
    [SerializeField] float separationRadius = 1.5f;
    [SerializeField] float separationWeight = 1f;
    [SerializeField] LayerMask enemyLayerMask;

    Rigidbody rb;
    Vector3 focusPoint;
    MoveMode moveMode = MoveMode.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void MoveTo(Vector3 destination)
    {
        focusPoint = destination;
        moveMode = MoveMode.Seeking;
    }

    public override void MoveAwayFrom(Vector3 position)
    {
        focusPoint = position;
        moveMode = MoveMode.Fleeing;
    }

    public override void Stop()
    {
        moveMode = MoveMode.Idle;
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        rb.angularVelocity = Vector3.zero;
    }

    public override void Face(Vector3 position)
    {
        Vector3 direction = position - rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            RotateTowards(direction);
    }

    void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            rb.angularVelocity = Vector3.zero;

            if (moveMode != MoveMode.Idle)
                Move();
        }
    }

    void Move()
    {
        Vector3 toFocus = focusPoint - rb.position;
        toFocus.y = 0f;

        bool arrived = moveMode == MoveMode.Seeking && toFocus.magnitude <= stoppingDistance;

        if (arrived)
            Stop();

        else
            Steer(toFocus);
    }

    void Steer(Vector3 toFocus)
    {
        Vector3 seekDirection = GetSeekDirection(toFocus);
        Vector3 moveDirection = (seekDirection + GetSeparationDirection() * separationWeight).normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (!ignoreMovementRotation)
            RotateTowards(seekDirection);
    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    Vector3 GetSeekDirection(Vector3 toFocus)
    {
        if (moveMode == MoveMode.Seeking)
            return toFocus.normalized;

        return toFocus.sqrMagnitude > 0.001f ? -toFocus.normalized : transform.right;
    }

    Vector3 GetSeparationDirection()
    {
        Collider[] neighbors = Physics.OverlapSphere(rb.position, separationRadius, enemyLayerMask);
        Vector3 push = Vector3.zero;

        foreach (Collider neighbor in neighbors)
        {
            Vector3 offset = rb.position - neighbor.attachedRigidbody.position;
            offset.y = 0f;

            float distance = offset.magnitude;

            if (neighbor.attachedRigidbody != rb && distance > 0.001f)
                push += offset.normalized * Mathf.Clamp01(1f - distance / separationRadius);
        }

        return push;
    }
}