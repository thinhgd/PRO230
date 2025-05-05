using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public static int totalStore = 0;

    public delegate void MeatChoppedEvent();
    public static event MeatChoppedEvent OnMeatChopped;
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Gather()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            //totalWood += maxHealth;

            InvokeGatheredEvent();

            MeatSpawner.Instance.RemoveMeat(this);
            MeatSpawner.Instance.RespawnMeat(this);

            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
    public void InvokeGatheredEvent()
    {
        OnMeatChopped?.Invoke();
    }
}
