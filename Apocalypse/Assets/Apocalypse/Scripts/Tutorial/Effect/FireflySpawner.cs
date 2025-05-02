using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflySpawner : MonoBehaviour
{
    public static FireflySpawner Instance;

    [Header("Spawn Settings")]
    public GameObject fireflyPrefab;
    public int count = 20;

    [Header("Spawn Area")]
    [SerializeField] private PolygonCollider2D spawnZone;
    public Color gizmoColor = Color.cyan;

    private List<GameObject> spawnedFireflies = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (fireflyPrefab == null || spawnZone == null)
        {
            return;
        }

        SpawnFireflies();
    }

    void SpawnFireflies()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomPointInPolygon();
            GameObject firefly = Instantiate(fireflyPrefab, pos, Quaternion.identity);
            spawnedFireflies.Add(firefly);
        }
    }

    Vector2 GetRandomPointInPolygon()
    {
        Bounds bounds = spawnZone.bounds;
        Vector2 point;
        int safety = 0;

        do
        {
            point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            safety++;
            if (safety > 1000)
            {
                break;
            }
        } while (!spawnZone.OverlapPoint(point));

        return point;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnZone != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(spawnZone.bounds.center, spawnZone.bounds.size);
        }
    }
}
