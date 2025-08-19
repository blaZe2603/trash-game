using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using System.Net;

[System.Serializable]
public class Trash
{
    public GameObject TrashPrefab;
    public float TrashInterval = 1f;
}

public class TrashSpawner : MonoBehaviour
{
    [SerializeField]float TrashSpawnRadius;
    [SerializeField]float TrashHeight;
    public Trash trash;
    public GameObject dustbinSpawnerGameObject;
    private DustbinSpawner DnSt;
    [SerializeField] private float timeForWaves = 5f;
    private bool isSpawning = false;

    void Start()
    {
        // StartCoroutine(TrashWaveSpawner());
        DnSt = dustbinSpawnerGameObject.GetComponent<DustbinSpawner>();

    }

    void Update()
    {
        // if (DnSt.aliveDustbins.Count == 0)
        // {
        //     if (DnSt.currWave < DnSt.waves.Length)
        //     {
        //         StartCoroutine(TrashWaveSpawner());
        //     }
        //     else
        //     {
        //         //complete
        //     }
        // }

        if (DnSt.aliveDustbins.Count != 0 && !isSpawning)
            StartCoroutine(SpawnTrash());
    }

    Vector3 PointInCamera(float fixedY = 2f)
    {
        Camera cam = Camera.main;

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        Vector3 camPos = cam.transform.position;

        float randX = Random.Range(-width / 2f, width / 2f);
        float randZ = Random.Range(-height / 2f, height / 2f);

        return new Vector3(camPos.x + randX, fixedY, camPos.z + randZ);
    }


    // IEnumerator TrashWaveSpawner()
    // {
    //     yield return new WaitForSeconds(timeForWaves);
    //     StartCoroutine(SpawnTrash(DnSt.waves[DnSt.currWave]));
    // }
    public Vector3 distance()
    {
        float angle = Random.Range(0, 360);
        float radius = Random.Range(0, TrashSpawnRadius);
        Vector3 spawnPoint = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0f, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
        return spawnPoint;
    }
    IEnumerator SpawnTrash()
    {
        isSpawning = true;

        while (DnSt.aliveDustbins.Count != 0)
        {
            Vector3 spawnPoint = distance() + new Vector3(0f,TrashHeight,0f);
            Instantiate(trash.TrashPrefab, spawnPoint, Quaternion.Euler(-90f, 0f, 0f));

            float delay = (1f / (DnSt.currWave + 1)) < 0.2f ? 0.2f : (1f / (DnSt.currWave + 1));
            yield return new WaitForSeconds(delay);
        }

        isSpawning = false; 
    }

    // IEnumerator NullCheck(GameObject dustbin)
    // {
    //     yield return new WaitUntil(() => dustbin == null);
    //     for (int i = aliveDustbins.Count - 1; i >= 0; i--)
    //     {
    //         if (aliveDustbins[i] == null)
    //         {
    //             aliveDustbins.RemoveAt(i);
    //         }
    //     }
    // }

}
