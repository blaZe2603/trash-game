using System.Collections;
using UnityEngine;

public class TrashType : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] float TimeToDestroy;

    private string[] typeTags = { "Hazardous", "General", "Wet", "Recyclable" };

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

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(TimeToDestroy);
        Destroy(gameObject);
    }
}
