using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class player_trash : MonoBehaviour
{
    [Header("GameObjects")]
    private Vector2 moveInput;
    private Rigidbody rb;
    Vector3 direction;
    private TrashPlayerActions trashInputActions;
    public LayerMask objectMask;

    [Header("Constants")]
    [SerializeField] float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        trashInputActions = new TrashPlayerActions();
        trashInputActions.Enable();

    }

    void FixedUpdate()
    {
        moveInput = trashInputActions.trash_move.move.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.sqrMagnitude > 1)
            direction.Normalize();
        rb.linearVelocity = direction * speed;

    }
        private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
