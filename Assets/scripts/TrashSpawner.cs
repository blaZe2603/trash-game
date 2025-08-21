using UnityEngine;
using System.Collections;

[System.Serializable]
public class Trash
{
    public GameObject[] TrashPrefabs;
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
        DnSt = dustbinSpawnerGameObject.GetComponent<DustbinSpawner>();

    }

    void Update()
    {
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
            int randomIndex = Random.Range(0, trash.TrashPrefabs.Length);
            GameObject selectedTrash = trash.TrashPrefabs[randomIndex];
            Vector3 spawnPoint = distance() + new Vector3(0f, TrashHeight, 0f);
            GameObject spawnedTrash = Instantiate(selectedTrash, spawnPoint, Quaternion.Euler(-90f, 0f, 0f));
            TrashType trashType = spawnedTrash.GetComponent<TrashType>();
            trashType.SetTrashType(randomIndex);

            float delay = (1f / (DnSt.currWave + 1)) < 0.2f ? 0.2f : (1f / (DnSt.currWave + 1));
            yield return new WaitForSeconds(delay);
        }

        isSpawning = false; 
    }
}
