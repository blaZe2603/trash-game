using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustbinSpawner : MonoBehaviour
{
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float waveDelay = 2f;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private GameObject dustbinPrefab;

    [SerializeField] private int startCount = 2;
    [SerializeField] private float multiplier = 2f;
    public int currWave = 0;

    private int currentWaveCount;
    public List<GameObject> aliveDustbins = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        currentWaveCount = startCount;
        // StartCoroutine(SpawnLoop());
        currWave = 0;

    }

    void Update()
    {
        aliveDustbins.RemoveAll(item => item == null);
        if (!isSpawning && aliveDustbins.Count == 0)
        {
            isSpawning = true;
            StartCoroutine(NextWave());
        }
    }


    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);
        yield return StartCoroutine(SpawnWave(currentWaveCount));
    }

    IEnumerator NextWave()
    {
        isSpawning = true;
        yield return new WaitForSeconds(waveDelay);
        currWave++;
        currentWaveCount = Mathf.CeilToInt(currentWaveCount * multiplier);
        yield return StartCoroutine(SpawnWave(currentWaveCount));
        isSpawning = false;
    }

    IEnumerator SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPoint = RandomPointOnCircle(spawnRadius);
            GameObject d = Instantiate(dustbinPrefab, spawnPoint, Quaternion.Euler(-90f, 0f, 0f));
            aliveDustbins.Add(d);
            Dustbin.BinType randType = (Dustbin.BinType)Random.Range(0, System.Enum.GetValues(typeof(Dustbin.BinType)).Length);
            d.GetComponent<Dustbin>().SetBinType(randType);
            Debug.Log("spawned");

            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
    }


    Vector3 RandomPointOnCircle(float radius)
    {
        float angle = Random.Range(0, 360);
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius
        );
    }

    IEnumerator NullCheck(GameObject dustbin)
    {
        yield return new WaitUntil(() => dustbin == null);
        aliveDustbins.RemoveAll(item => item == null);
    }
}
