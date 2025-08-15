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
        if (collision.collider.gameObject.tag == GetComponent<Renderer>().material.color.ToString())
        {
            Damage(1);
        }
    }

    private Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
        return colors[Random.Range(0, colors.Length)];

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
