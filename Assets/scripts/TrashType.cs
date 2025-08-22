using System.Collections;
using UnityEngine;

public class TrashType : MonoBehaviour
{
    private Coroutine destroyCoroutine;
    private int originalLayer;
    private Renderer rend;
    [SerializeField] float TimeToDestroy;

    private string[] typeTags = { "Hazardous", "General", "Wet", "Recyclable" };

    public bool isThrown { get; private set; } = false;

    void Start()
    {
    rend = GetComponent<Renderer>();
    originalLayer = gameObject.layer;
    destroyCoroutine = StartCoroutine(DestroyAfterTime());

    }
    
    void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("thrownTrash"))
        {
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }
        else if (destroyCoroutine == null && gameObject.layer != LayerMask.NameToLayer("thrownTrash"))
        {
            destroyCoroutine = StartCoroutine(DestroyAfterTime());
        }
    }

    public void SetTrashType(int index)
    {
        gameObject.tag = typeTags[index];
        Debug.Log($"Spawned {gameObject.tag} trash");
    }

    public void MarkAsThrown()
    {
        isThrown = true;
        gameObject.layer = LayerMask.NameToLayer("thrownTrash");
        StartCoroutine(ChangeLayer());
        
    }
    IEnumerator ChangeLayer()
    {
        yield return new WaitForSeconds(1f);
        gameObject.layer = LayerMask.NameToLayer("objects");
    }
    IEnumerator DestroyAfterTime()
    {
        float elapsed = 0f;

        while (elapsed < TimeToDestroy)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
