using System.Collections;
using UnityEngine;

public class TrashType : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] float TimeToDestroy;

    private string[] typeTags = { "Hazardous", "General", "Wet", "Recyclable" };

    public bool isThrown { get; private set; } = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        StartCoroutine(DestroyAfterTime());
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
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(TimeToDestroy);
        Destroy(gameObject);
    }
}
