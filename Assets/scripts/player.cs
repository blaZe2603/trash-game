using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    Vector3 direction;
    [Header("Constants")]
    [SerializeField] float objectDetectRadius;
    [SerializeField] float speed;
    public float minPower = 5f;
    public float maxPower = 20f;
    public float chargeSpeed = 10f;
    public int maxHealth = 6;
    private int currHealth;
    public float invincibilityTime = 1f;
    private float invincibilityTimer;

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

        currHealth = maxHealth;
        invincibilityTimer = 0;
    }

    void Update()
    {
        if (currHealth <= 0)
        {
            SceneManager.LoadScene(0);
        }
        if (invincibilityTimer >= 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else
        {
            invincibilityTimer = 0F;
        }
        Debug.Log(currHealth);

    }

    void FixedUpdate()
    {
        // Player movement
        moveInput = playerInput.player.move.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y);
        
        if (math.abs(direction.x) > 0)
            holdPoint.localPosition = new Vector3(direction.x*2,0f,0f);

        else if (math.abs(direction.z) > 0)
            holdPoint.localPosition = new Vector3(0f,0f,direction.z*2);

        if (direction.sqrMagnitude > 0.01f)
        {
            // place holdPoint in front of player
            holdPoint.localPosition = direction.normalized * 2f;

            // rotate holdPoint to face the direction
            holdPoint.forward = direction.normalized;
        }


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
            heldRb.linearVelocity = holdPoint.forward * currentPower;
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

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Recyclable Bin") ||
        collision.collider.CompareTag("Wet Bin") ||
        collision.collider.CompareTag("General Bin") ||
        collision.collider.CompareTag("Hazardous Bin"))
        {
            if (invincibilityTimer == 0)
            {
                currHealth -= 1;
                invincibilityTimer = invincibilityTime;
                collision.collider.gameObject.GetComponent<Dustbin>().Damage(1);
            }
        }
    }
}
