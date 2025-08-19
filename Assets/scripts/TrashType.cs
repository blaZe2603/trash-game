using System.Collections;
using UnityEngine;

public class TrashType : MonoBehaviour
{

    private Renderer rend;

    [SerializeField] float TimeToDestroy;
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = GetRandomColor();

        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(TimeToDestroy);
        Destroy(gameObject);
    }

    private Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
        Color toRet = colors[Random.Range(0, colors.Length)];
        if (toRet == Color.red)
        {
            gameObject.tag = "Hazardous";
        }
        if (toRet == Color.blue)
        {
            gameObject.tag = "General";
        }
        if (toRet == Color.green)
        {
            gameObject.tag = "Wet";
        }
        if (toRet == Color.yellow)
        {
            gameObject.tag = "Recyclable";
        }
        Debug.Log(gameObject.tag);
        return toRet;
    }
}
