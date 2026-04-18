using UnityEngine;

public class abr : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float stoppingDistance = 0.1f;
    public float stuckTimeout = 1.2f;
    public float progressEpsilon = 0.01f;

    [Header("Debug")]
    public bool enableKeyboardDebugMovement = false;

    [Header("Physics Tuning")]
    public float rbMass = 20f;
    public float rbDrag = 8f;

    private Rigidbody rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector3 movement;
    private bool hasMoveOrder;
    private Vector3 moveTarget;
    private float previousDistanceToTarget = -1f;
    private float stuckTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();           
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogWarning("[UNIDAD] abr necesita un Rigidbody para moverse.");
            enabled = false;
            return;
        }

        rb.freezeRotation = true;
        rb.isKinematic = false;
        rb.mass = rbMass;
        rb.linearDamping = rbDrag;
        rb.angularDamping = 999f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void Update()
    {
        if (enableKeyboardDebugMovement)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            movement = new Vector3(moveX, 0, moveZ).normalized;
            hasMoveOrder = false;
            previousDistanceToTarget = -1f;
            stuckTimer = 0f;
        }
        else if (hasMoveOrder)
        {
            Vector3 toTarget = moveTarget - rb.position;
            toTarget.y = 0f;
            float currentDistance = toTarget.magnitude;

            if (previousDistanceToTarget >= 0f)
            {
                float progress = previousDistanceToTarget - currentDistance;
                if (progress > progressEpsilon)
                {
                    stuckTimer = 0f;
                }
                else
                {
                    stuckTimer += Time.deltaTime;
                }
            }

            previousDistanceToTarget = currentDistance;

            if (toTarget.sqrMagnitude <= stoppingDistance * stoppingDistance)
            {
                CancelMoveOrder();
            }
            else
            {
                movement = toTarget.normalized;

                if (stuckTimer >= stuckTimeout)
                {
                    Debug.Log($"<color=orange>[UNIDAD]</color> {name} atascada por obstaculo. Cancelando orden de movimiento.");
                    CancelMoveOrder();
                }
            }
        }
        else
        {
            movement = Vector3.zero;
        }

        if (anim != null)
        {
            bool isMoving = movement.magnitude > 0;
            anim.SetBool("isWalking", isMoving);
        }

        if (spriteRenderer != null)
        {
            UpdateFacing(movement);
        }
    }

    void FixedUpdate()
    {
        if (movement.magnitude > 0.1f)
        {
            Vector3 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    public void SetMoveTarget(Vector3 target)
    {
        moveTarget = target;
        moveTarget.y = rb != null ? rb.position.y : transform.position.y;
        hasMoveOrder = true;
        previousDistanceToTarget = -1f;
        stuckTimer = 0f;
    }

    private void CancelMoveOrder()
    {
        movement = Vector3.zero;
        hasMoveOrder = false;
        previousDistanceToTarget = -1f;
        stuckTimer = 0f;
    }

    private void UpdateFacing(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        if (Mathf.Abs(direction.z) > Mathf.Abs(direction.x))
        {
            spriteRenderer.flipX = direction.z > 0f;
        }
        else
        {
            spriteRenderer.flipX = direction.x < 0f;
        }
    }
}   