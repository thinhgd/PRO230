using UnityEngine;

public class WorkerSpawner : MonoBehaviour
{
    public GameObject workerPrefab;
    public Transform spawnPoint;

    public void SpawnWorker()
    {
        Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);
    }
}
