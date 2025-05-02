using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public static TreeSpawner Instance;

    [Header("Spawn Settings")]
    public int maxTreeCount = 20;
    private List<Tree> spawnedTrees = new List<Tree>();

    [SerializeField] private GameObject treePrefab;

    [Header("Spawn Area")]
    [SerializeField] private PolygonCollider2D spawnZone;
    public Color gizmoColor = Color.yellow;

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

        SpawnTrees();
    }

    public void RespawnTree(Tree tree)
    {
        StartCoroutine(RespawnTreeCoroutine(tree));
    }

    private IEnumerator RespawnTreeCoroutine(Tree tree)
    {
        yield return new WaitForSeconds(30f);

        if (spawnedTrees.Count < maxTreeCount)
        {
            Vector2 randomPos = GetRandomPointInPolygon();

            tree.transform.position = randomPos;
            tree.gameObject.SetActive(true);
            tree.currentHealth = tree.maxHealth;

            spawnedTrees.Add(tree);
        }
    }

    private void SpawnTrees()
    {
        for (int i = 0; i < maxTreeCount; i++)
        {
            Tree tree = InstantiateTree();
            spawnedTrees.Add(tree);
        }
    }

    private Tree InstantiateTree()
    {
        Vector2 randomPos = GetRandomPointInPolygon();

        GameObject treeObject = Instantiate(treePrefab, randomPos, Quaternion.identity);
        Tree tree = treeObject.GetComponent<Tree>();
        return tree;
    }

    public void RemoveTree(Tree tree)
    {
        spawnedTrees.Remove(tree);
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
