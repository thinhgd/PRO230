using System;
using UnityEngine;

public enum UnitType { Farmer, Miner }
public enum ResourceType { Wood, Stone }

public class Unit : MonoBehaviour
{
    public UnitType unitType;
    public Action onDeath;

    //private int hp = 100;

    void Update()
    {
        // hp -= 1;
        // if (hp <= 0)
        // {
        //     onDeath?.Invoke();
        //     Destroy(gameObject);
        // }
    }
    public void Gathered(ResourceType type, int amount)
    {
        FindObjectOfType<CastleManager>().AddResource(type, amount);
    }
}
