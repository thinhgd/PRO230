using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Linq;

public class CastleManager : MonoBehaviour
{
    public enum CastleLevel { Level1, Level2, Level3, Level4, Level5 };
    public CastleLevel currentLevel = CastleLevel.Level1;

    public Transform spawnPoint;
    public GameObject farmerPrefab;
    public GameObject minerPrefab;
    public GameObject builderPrefab;
    public Transform builderSpawnPoint;

    public GameObject farmPrefab;
    public GameObject storagePrefab;
    public GameObject constructionPrefab;
    public Transform buildingParent;
    [Header("Spawn Area for Farms")]
    [SerializeField] private PolygonCollider2D farmSpawnZone;
    public Color gizmoColor = Color.green;


    public int wood = 0;
    public int stone = 0;

    private List<GameObject> farmers = new List<GameObject>();
    private List<GameObject> miners = new List<GameObject>();
    private List<BuilderAI> allBuilders = new List<BuilderAI>();
    private List<GameObject> farms = new List<GameObject>();
    private List<GameObject> storages = new List<GameObject>();

    private float checkInterval = 2f;
    private float timer = 0f;

    public Transform castleParent;
    public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;
    public GameObject level4Prefab;

    private GameObject currentCastleObject;

    void Start()
    {
        ReplaceCastleVisual(currentLevel);
        ReScanPathfinder();
        EnsureBuilderCount();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            HandleLogic();
        }
    }

    void HandleLogic()
    {
        switch (currentLevel)
        {
            case CastleLevel.Level1:
                if (farmers.Count < 3) SpawnFarmer();

                BuildLevel1();

                if (farms.Count >= 5 && wood >= 50)
                {
                    wood -= 50;
                    UpgradeCastle(CastleLevel.Level2);
                }
                break;

            case CastleLevel.Level2:
                if (farmers.Count < 5) SpawnFarmer();
                else if (miners.Count < 3) SpawnMiner();

                BuildLevel2();

                if (farms.Count >= 20 && storages.Count >= 20 && wood >= 100 && stone >= 50)
                {
                    wood -= 100;
                    stone -= 50;
                    UpgradeCastle(CastleLevel.Level3);
                }
                break;

            case CastleLevel.Level3:
                if (farmers.Count < 6) SpawnFarmer();
                else if (miners.Count < 4) SpawnMiner();

                if (wood >= 300 && stone >= 150)
                {
                    wood -= 300;
                    stone -= 150;
                    UpgradeCastle(CastleLevel.Level4);
                }
                break;

            case CastleLevel.Level4:
                break;
        }

        CheckRespawn();
    }

    void BuildLevel1()
    {
        if (farms.Count < 5 && wood >= 20)
        {
            wood -= 20;
            BuildFarm();
        }
    }

    void BuildLevel2()
    {
        if (farms.Count < 20 && wood >= 20)
        {
            wood -= 20;
            BuildFarm();
        }
        else if (storages.Count < 2 && wood >= 30 && stone >= 10)
        {
            wood -= 30;
            stone -= 10;
            BuildStorage();
        }
    }

    void BuildFarm()
    {
        Vector3? pos = FindBuildPositionForFarm();
        if (pos.HasValue)
        {
            Vector3 position = pos.Value;

            GameObject construction = Instantiate(constructionPrefab, position, Quaternion.identity, buildingParent);
            ConstructionProgress cp = construction.GetComponent<ConstructionProgress>();
            cp.finalBuildingPrefab = farmPrefab;
            cp.parentOnComplete = buildingParent;
            cp.StartConstruction(position);

            farms.Add(construction);
            ReScanPathfinder();

            Debug.Log("Started building Farm");
        }
    }



    void BuildStorage()
    {
        Vector3? pos = FindBuildPositionForFarm();
        if (pos.HasValue)
        {
            GameObject storage = Instantiate(storagePrefab, pos.Value, Quaternion.identity, buildingParent);
            storages.Add(storage);
            ReScanPathfinder();
            Debug.Log("Built Storage");
        }
    }

    void SpawnFarmer()
    {
        GameObject farmer = Instantiate(farmerPrefab, spawnPoint.position, Quaternion.identity);
        farmer.GetComponent<Unit>().onDeath += () => OnUnitDeath(farmer, UnitType.Farmer);
        farmers.Add(farmer);
    }

    void SpawnMiner()
    {
        GameObject miner = Instantiate(minerPrefab, spawnPoint.position, Quaternion.identity);
        miner.GetComponent<Unit>().onDeath += () => OnUnitDeath(miner, UnitType.Miner);
        miners.Add(miner);
    }
    private void SpawnBuilder()
    {
        GameObject builderObj = Instantiate(builderPrefab, builderSpawnPoint.position, Quaternion.identity);
        BuilderAI builder = builderObj.GetComponent<BuilderAI>();

        GameObject home = new GameObject("BuilderHome");
        home.transform.position = builderSpawnPoint.position;
        builder.homePosition = home.transform;

        allBuilders.Add(builder);
    }

    private int GetMaxBuilderByLevel()
    {
        switch (currentLevel)
        {
            case CastleLevel.Level1: return 1;
            case CastleLevel.Level2: return 2;
            case CastleLevel.Level3: return 3;
            case CastleLevel.Level4: return 4;
            default: return 1;
        }
    }
    private void EnsureBuilderCount()
    {
        int maxBuilder = GetMaxBuilderByLevel();
        while (allBuilders.Count < maxBuilder)
        {
            SpawnBuilder();
        }
    }
    void OnUnitDeath(GameObject unit, UnitType type)
    {
        if (type == UnitType.Farmer) farmers.Remove(unit);
        if (type == UnitType.Miner) miners.Remove(unit);
    }

    void CheckRespawn()
    {
        switch (currentLevel)
        {
            case CastleLevel.Level1:
                if (farmers.Count < 1 ) SpawnFarmer();
                break;
            case CastleLevel.Level2:
                if (farmers.Count < 2) SpawnFarmer();
                else if (miners.Count < 1) SpawnMiner();
                break;
            case CastleLevel.Level3:
                if (farmers.Count < 3) SpawnFarmer();
                else if (miners.Count < 2) SpawnMiner();
                break;
            case CastleLevel.Level4:
                if (farmers.Count < 4) SpawnFarmer();
                else if (miners.Count < 3) SpawnMiner();
                break;
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Wood) wood += amount;
        if (type == ResourceType.Stone) stone += amount;
    }

    void UpgradeCastle(CastleLevel newLevel)
    {
        currentLevel = newLevel;
        Debug.Log($"Castle upgraded to {newLevel}!");
        ReplaceCastleVisual(newLevel);
        ReScanPathfinder();
        EnsureBuilderCount();
    }

    void ReplaceCastleVisual(CastleLevel newLevel)
    {
        if (currentCastleObject != null) Destroy(currentCastleObject);

        GameObject prefab = null;
        switch (newLevel)
        {
            case CastleLevel.Level1: prefab = level1Prefab; break;
            case CastleLevel.Level2: prefab = level2Prefab; break;
            case CastleLevel.Level3: prefab = level3Prefab; break;
            case CastleLevel.Level4: prefab = level4Prefab; break;
        }

        if (prefab != null)
            currentCastleObject = Instantiate(prefab, castleParent.position, Quaternion.identity, castleParent);
    }

    void ReScanPathfinder()
    {
        StartCoroutine(DelayedGraphUpdate());
    }

    IEnumerator DelayedGraphUpdate()
    {
        yield return null;
        Collider2D[] colliders = currentCastleObject.GetComponentsInChildren<Collider2D>();
        yield return new WaitUntil(() => colliders.All(c => c.enabled && c.bounds.size.sqrMagnitude > 0.01f));

        foreach (Collider2D col in colliders)
        {
            Bounds bounds = col.bounds;
            GraphUpdateObject guo = new GraphUpdateObject(bounds);
            guo.updatePhysics = true;
            guo.modifyWalkability = true;
            guo.setWalkability = false;
            AstarPath.active.UpdateGraphs(guo);
        }
    }

    Vector3? FindBuildPositionForFarm()
    {
        int maxAttempts = 10;  // So lan tim kiem vi tri hop le
        float spawnRadius = 20f;  // Ban kinh tim kiem xung quanh castle

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                Random.Range(-spawnRadius, spawnRadius),
                0
            );

            Vector3 worldPos = castleParent.position + randomOffset;

            if (farmSpawnZone.OverlapPoint(worldPos))
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(worldPos, new Vector2(1f, 1f), 0);
                bool isBlocked = colliders.Length > 0 && !colliders.Any(c => c == farmSpawnZone);

                if (!isBlocked)
                {
                    return worldPos;
                }
            }
        }
        return null;
    }

}
