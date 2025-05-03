using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    public static StoneSpawner Instance;

    [Header("Spawn Settings")]
    public int maxTreeCount = 20;
    private List<Stone> spawnedTrees = new List<Stone>();

    [SerializeField] private GameObject treePrefab;

    [Header("Spawn Area")]
    [SerializeField] private PolygonCollider2D spawnZone;
    public Color gizmoColor = Color.yellow;
    private Transform stoneContainer;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (treePrefab == null || spawnZone == null)
        {
            return;
        }
        stoneContainer = new GameObject("StoneContainer").transform;
        SpawnTrees();
    }

    public void RespawnStone(Stone stone)
    {
        StartCoroutine(RespawnTreeCoroutine(stone));
    }

    private IEnumerator RespawnTreeCoroutine(Stone stone)
    {
        yield return new WaitForSeconds(70f);

        if (spawnedTrees.Count < maxTreeCount)
        {
            Vector2 randomPos = GetRandomPointInPolygon();

            stone.transform.position = randomPos;
            stone.transform.SetParent(stoneContainer);
            stone.gameObject.SetActive(true);
            stone.currentHealth = stone.maxHealth;

            spawnedTrees.Add(stone);
        }
    }

    private void SpawnTrees()
    {
        for (int i = 0; i < maxTreeCount; i++)
        {
            Stone stone = InstantiateTree();
            spawnedTrees.Add(stone);
        }
    }

    private Stone InstantiateTree()
    {
        Vector2 randomPos = GetRandomPointInPolygon();

        GameObject stoneObject = Instantiate(treePrefab, randomPos, Quaternion.identity, stoneContainer);
        Stone stone = stoneObject.GetComponent<Stone>();
        return stone;
    }

    public void RemoveStone(Stone stone)
    {
        spawnedTrees.Remove(stone);
    }

    private Vector2 GetRandomPointInPolygon()
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
