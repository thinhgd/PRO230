using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatSpawner : MonoBehaviour
{
    public static MeatSpawner Instance;

    [Header("Spawn Settings")]
    public int maxMeatCount = 20;
    private List<Meat> spawnedMeats = new List<Meat>();

    [SerializeField] private GameObject meatPrefab;

    [Header("Spawn Area")]
    [SerializeField] private PolygonCollider2D spawnZone;
    public Color gizmoColor = Color.yellow;

    private Transform meatContainer;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (meatPrefab == null || spawnZone == null)
        {
            return;
        }
        meatContainer = new GameObject("MeatContainer").transform;
        SpawnMeats();
    }

    public void RespawnMeat(Meat meat)
    {
        StartCoroutine(RespawnMeatCoroutine(meat));
    }

    private IEnumerator RespawnMeatCoroutine(Meat meat)
    {
        yield return new WaitForSeconds(30f);

        if (spawnedMeats.Count < maxMeatCount)
        {
            Vector2 randomPos = GetRandomPointInPolygon();

            meat.transform.position = randomPos;
            meat.transform.SetParent(meatContainer);
            meat.gameObject.SetActive(true);
            meat.currentHealth = meat.maxHealth;

            spawnedMeats.Add(meat);
        }
    }

    private void SpawnMeats()
    {
        for (int i = 0; i < maxMeatCount; i++)
        {
            Meat meat = InstantiateMeat();
            spawnedMeats.Add(meat);
        }
    }

    private Meat InstantiateMeat()
    {
        Vector2 randomPos = GetRandomPointInPolygon();

        GameObject meatObject = Instantiate(meatPrefab, randomPos, Quaternion.identity, meatContainer);
        Meat meat = meatObject.GetComponent<Meat>();
        return meat;
    }

    public void RemoveMeat(Meat meat)
    {
        spawnedMeats.Remove(meat);
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
