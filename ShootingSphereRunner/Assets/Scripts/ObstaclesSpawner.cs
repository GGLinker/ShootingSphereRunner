using UnityEngine;
using Random = System.Random;

public class ObstaclesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField, Range(0, 100)] private int fillRate;

    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Transform temple;
    
    [SerializeField] public float minSpawnDistance;
    [SerializeField] public float templeFreeDistance;
    [SerializeField] public float widthSpawnBounds;

    public float DistanceToTemple => Vector3.Distance(temple.position - Vector3.forward * templeFreeDistance, bulletSpawn.position);

    private void Start()
    {
        var bulletSpawnBound = bulletSpawn.position.z + minSpawnDistance;
        var templeSpawnBound = temple.position.z - temple.localScale.z / 2 - templeFreeDistance;
        
        float area = widthSpawnBounds * (templeSpawnBound - bulletSpawnBound);
        int needAmount = (int)(area / ((100f / fillRate) * Mathf.Pow(obstaclePrefab.transform.localScale.x * 1.5f, 2)));
        int spawnedAmount = 0;

        while (spawnedAmount < needAmount)
        {
            Random rand = new Random();
            Instantiate(obstaclePrefab, 
                new Vector3(
                    (float)(rand.NextDouble() * widthSpawnBounds) - widthSpawnBounds / 2,
                    obstaclePrefab.transform.position.y,
                    (float)(rand.NextDouble() * (templeSpawnBound - bulletSpawnBound)) + bulletSpawnBound),
                Quaternion.identity);
            spawnedAmount++;
        }
    }
}
