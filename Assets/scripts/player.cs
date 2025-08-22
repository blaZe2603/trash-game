using System.Xml.Serialization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("GameObjects")]
    audio_manager audio_Manager;
    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInputActions playerInput;
    public Transform holdPoint;
    private GameObject heldObject;
    private Rigidbody heldRb;
    public LayerMask objectMask;
    private Animator animator;
    Vector3 direction;
    [Header("Constants")]
    [SerializeField] float objectDetectRadius;
    [SerializeField] private float throwPower = 15f;
    [SerializeField] float speed;
    public float minPower = 5f;
    public float maxPower = 20f;
    public float chargeSpeed = 10f;
    public int maxHealth = 6;
    private int currHealth;
    public float invincibilityTime = 1f;
    private float invincibilityTimer;
    public int score;

    private float currentPower;
    private bool isCharging;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        try
        {
            audio_Manager = GameObject.FindGameObjectWithTag("audio").GetComponent<audio_manager>();
        }
        catch
        {
            Debug.Log("hi");
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        playerInput = new PlayerInputActions();
        playerInput.Enable();

        playerInput.player.collect.performed += CollectObject;

        currHealth = maxHealth;
        invincibilityTimer = 0;
        score = 0;
    }

    void Update()
    {
        if (currHealth <= 0)
        {
            audio_Manager.PlaySound(audio_Manager.death);
            SceneManager.LoadSceneAsync(3);
        }
        if (invincibilityTimer >= 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else
        {
            invincibilityTimer = 0F;
        }
        // Debug.Log(currHealth);
        healthText.text = "Health : " + currHealth;
        scoreText.text = "Score : " + score;


    }

    void FixedUpdate()
    {
        // Player movement
        moveInput = playerInput.player.move.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y);

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }

        // Update holdPoint position and rotation relative to player
        Vector3 offset = new Vector3(0, 0f, 3f);
        holdPoint.position = transform.position + transform.TransformDirection(offset);
        holdPoint.rotation = transform.rotation;

        // Apply player movement
        rb.linearVelocity = direction * speed;
        float moveAmount = direction.magnitude;
        animator.SetFloat("MoveSpeed", moveAmount);

        // Move held object along with player
        if (heldObject != null && heldRb != null)
        {
            Vector3 targetPos = holdPoint.position;
            Vector3 moveDir = (targetPos - heldRb.position) / Time.fixedDeltaTime;
            heldRb.linearVelocity = moveDir;
        }

        // Charging throw logic
        if (isCharging && heldObject != null)
        {
            currentPower += chargeSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
        }
    }


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
                heldObject.GetComponent<TrashType>().MarkAsThrown();
            }
        }
        else
        {
            ThrowHeldObject();
        }
    }
    // void LateUpdate()
    // {
    //     holdPoint.rotation = Quaternion.identity;
    //     holdPoint.localPosition = new Vector3(1, 0, 0);
    // }


    private void ThrowHeldObject()
    {
        if (heldObject == null || heldRb == null) return;

        heldRb.linearVelocity = holdPoint.forward * throwPower;

        heldObject.GetComponent<TrashType>().MarkAsThrown();
        animator.SetTrigger("isThrowing");

        heldObject = null;
        heldRb = null;
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
                try
                {
                    audio_Manager.PlaySound(audio_Manager.player_hit);
                }
                catch
                {
                    Debug.Log("Its okay");
                }
                invincibilityTimer = invincibilityTime;
                collision.collider.gameObject.GetComponent<Dustbin>().Damage(1);
            }
        }
    }
}
