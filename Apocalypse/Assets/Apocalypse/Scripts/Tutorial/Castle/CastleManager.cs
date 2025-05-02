using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class CastleManager : MonoBehaviour
{
    public enum CastleLevel { Level1, Level2, Level3, Level4, Level5, Level6 };
    public CastleLevel currentLevel = CastleLevel.Level1;

    public Transform spawnPoint;
    public GameObject farmerPrefab;
    public GameObject minerPrefab;

    public int wood = 0;
    public int stone = 0;

    private List<GameObject> farmers = new List<GameObject>();
    private List<GameObject> miners = new List<GameObject>();

    private float checkInterval = 2f;
    private float timer = 0f;

    // public SpriteRenderer castleRenderer;
    // public Sprite level1Sprite;
    // public Sprite level2Sprite;
    // public Sprite level3Sprite;
    public Transform castleParent;
    public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    private GameObject currentCastleObject;

    void Start()
    {
        ReplaceCastleVisual(currentLevel);
        ReScanPathfinder();
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
                if (farmers.Count < 3)
                {
                    SpawnFarmer();
                }
                if (farmers.Count == 3 && wood >= 50)
                {
                    wood -= 50;
                    currentLevel = CastleLevel.Level2;
                    Debug.Log("Castle upgraded to Level 2!");
                    ReplaceCastleVisual(currentLevel);
                    ReScanPathfinder();
                }
                break;

            case CastleLevel.Level2:
                if (farmers.Count < 5)
                {
                    SpawnFarmer();
                }
                else if (miners.Count < 3)
                {
                    SpawnMiner();
                }
                else if (wood >= 100 && stone >= 50)
                {
                    currentLevel = CastleLevel.Level3;
                    Debug.Log("Castle upgraded to Level 3!");
                    ReplaceCastleVisual(currentLevel);
                    ReScanPathfinder();
                }
                break;

            case CastleLevel.Level3:
                if (farmers.Count < 6)
                {
                    SpawnFarmer();
                }
                else if (miners.Count < 4)
                {
                    SpawnMiner();
                }
                else if (wood >= 300 && stone >= 150)
                {
                    currentLevel = CastleLevel.Level4;
                    Debug.Log("Castle upgraded to Level 4!");
                    ReplaceCastleVisual(currentLevel);
                    ReScanPathfinder();
                }
                break;
            case CastleLevel.Level4:
                if (farmers.Count < 8)
                {
                    SpawnFarmer();
                }
                else if (miners.Count < 5)
                {
                    SpawnMiner();
                }
                else if (wood >= 1300 && stone >= 1150)
                {
                    currentLevel = CastleLevel.Level4;
                    Debug.Log("Castle upgraded to Level 5!");
                    ReplaceCastleVisual(currentLevel);
                    ReScanPathfinder();
                }
                break;
        }

        CheckRespawn();
    }

    void SpawnFarmer()
    {
        GameObject farmer = Instantiate(farmerPrefab, spawnPoint.position, Quaternion.identity);
        farmer.GetComponent<Unit>().onDeath += () => OnUnitDeath(farmer, UnitType.Farmer);
        farmers.Add(farmer);
        ReScanPathfinder();
    }

    void SpawnMiner()
    {
        GameObject miner = Instantiate(minerPrefab, spawnPoint.position, Quaternion.identity);
        miner.GetComponent<Unit>().onDeath += () => OnUnitDeath(miner, UnitType.Miner);
        miners.Add(miner);
        ReScanPathfinder();
    }

    void OnUnitDeath(GameObject unit, UnitType type)
    {
        if (type == UnitType.Farmer) farmers.Remove(unit);
        if (type == UnitType.Miner) miners.Remove(unit);
    }

    void CheckRespawn()
    {
        if (currentLevel == CastleLevel.Level1 && farmers.Count < 1)
            SpawnFarmer();

        if (currentLevel == CastleLevel.Level2 && farmers.Count < 2)
            SpawnFarmer();

        if (currentLevel == CastleLevel.Level2 && farmers.Count >= 2 && miners.Count < 1)
            SpawnMiner();
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Wood) wood += amount;
        if (type == ResourceType.Stone) stone += amount;
    }
    void ReplaceCastleVisual(CastleLevel newLevel)
    {
        if (currentCastleObject != null)
        {
            Destroy(currentCastleObject);
        }

        switch (newLevel)
        {
            case CastleLevel.Level1:
                currentCastleObject = Instantiate(level1Prefab, castleParent.position, Quaternion.identity, castleParent);
                break;
            case CastleLevel.Level2:
                currentCastleObject = Instantiate(level2Prefab, castleParent.position, Quaternion.identity, castleParent);
                break;
            case CastleLevel.Level3:
                currentCastleObject = Instantiate(level3Prefab, castleParent.position, Quaternion.identity, castleParent);
                break;
        }
    }
    void ReScanPathfinder()
    {
        AstarPath.active.Scan();
    }
}
