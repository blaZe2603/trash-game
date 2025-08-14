using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("GameObjects")]
    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInputActions playerInput;
    public Transform holdPoint;
    private GameObject heldObject;
    private Rigidbody heldRb;
    public LayerMask objectMask;


    [Header("Constants")]
    [SerializeField] float objectDetectRadius;
    [SerializeField] float speed;
    public float minPower = 5f;
    public float maxPower = 20f;
    public float chargeSpeed = 10f;

    private float currentPower;
    private bool isCharging;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerInput = new PlayerInputActions();
        playerInput.Enable();

        playerInput.player.collect.performed += CollectObject;
        playerInput.player.shoot.started += ChargeShoot;
        playerInput.player.shoot.canceled += ReleaseShot;
    }

    void FixedUpdate()
    {
        // Player movement
        moveInput = playerInput.player.move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        rb.linearVelocity = direction * speed;

        // Move held object along with player
        if (heldObject != null && heldRb != null)
        {
            Vector3 targetPos = holdPoint.position;
            Vector3 moveDir = (targetPos - heldRb.position) / Time.fixedDeltaTime;
            heldRb.linearVelocity = moveDir;
        }

        // Charge power while holding shoot
        if (isCharging && heldObject != null)
        {
            currentPower += chargeSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
        }
    }

    // collecting near objects
    public void CollectObject(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (heldObject == null)
        {
            heldObject = Near();
            if (heldObject != null)
            {
                heldRb = heldObject.GetComponent<Rigidbody>();
                heldRb.isKinematic = false;
            }
        }
        else
        {
            heldObject = null;
            heldRb = null;
        }
    }

    //if started shooting then set this to true so can start powering the shot
    public void ChargeShoot(InputAction.CallbackContext context)
    {
        if (!isCharging)
            isCharging = true;

        currentPower = minPower;
    }

    // shooting if we have an object
    public void ReleaseShot(InputAction.CallbackContext context)
    {
        if (!isCharging) return;

        isCharging = false;

        if (heldObject != null && heldRb != null)
        {
            // Shoot nad set heldobject to null
            heldRb.linearVelocity = new Vector3(1f, 1f, 0f) * currentPower;
            heldObject = null;
            heldRb = null;
        }
    }

    public GameObject Near()
    {
        float minDist = float.PositiveInfinity;
        GameObject nearestObject = null;

        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            objectDetectRadius,
            objectMask
        );

        foreach (var hitCollider in hitColliders)
        {
            float dist = (hitCollider.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearestObject = hitCollider.gameObject;
            }
        }

        return nearestObject;
    }
}
