using UnityEngine;

public class Dustbin : MonoBehaviour
{
    public enum BinType
    {
        Basic,
        ColorChanger
    }

    [SerializeField] private float speed = 3f;
    [SerializeField] private float eps = 5f;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float colorChangeInterval = 2f;
    private int currHealth;

    private Rigidbody rb;
    private Transform player;
    private float colorTimer;

    public BinType binType { get; private set; }
    private Renderer rend;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currHealth = maxHealth;

        if (binType == BinType.Basic)
        {
            rend.material.color = GetRandomColor();
        }
        else if (binType == BinType.ColorChanger)
        {
            rend.material.color = GetRandomColor();
            colorTimer = colorChangeInterval;
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.magnitude > eps)
            rb.linearVelocity = dir.normalized * speed;
        else
            rb.linearVelocity = Vector3.zero;

        if (binType == BinType.ColorChanger)
        {
            colorTimer -= Time.fixedDeltaTime;
            if (colorTimer <= 0f)
            {
                rend.material.color = GetRandomColor();
                colorTimer = colorChangeInterval;
            }
        }
    }

    public void SetBinType(BinType type)
    {
        binType = type;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Hazardous") &&
            !collision.collider.CompareTag("General") &&
            !collision.collider.CompareTag("Wet") &&
            !collision.collider.CompareTag("Recyclable"))
            return;

        TrashType trash = collision.collider.GetComponent<TrashType>();
        if (trash == null) return;

        if (trash.isThrown)
        {
            if ((collision.collider.CompareTag("Hazardous") && CompareTag("Hazardous Bin")) ||
                (collision.collider.CompareTag("General") && CompareTag("General Bin")) ||
                (collision.collider.CompareTag("Wet") && CompareTag("Wet Bin")) ||
                (collision.collider.CompareTag("Recyclable") && CompareTag("Recyclable Bin")))
            {
                Damage(1);
                Destroy(collision.gameObject);
            }
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }

    
    private Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
        Color toRet = colors[Random.Range(0, colors.Length)];
        if (toRet == Color.red)
        {
            gameObject.tag = "Hazardous Bin";
        }
        if (toRet == Color.blue)
        {
            gameObject.tag = "General Bin";
        }
        if (toRet == Color.green)
        {
            gameObject.tag = "Wet Bin";
        }
        if (toRet == Color.yellow)
        {
            gameObject.tag = "Recyclable Bin";
        }
        // Debug.Log(gameObject.tag);
        return toRet;

    }

    public void Damage(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            Destroy(gameObject);
        }
        //ok
    }
}
