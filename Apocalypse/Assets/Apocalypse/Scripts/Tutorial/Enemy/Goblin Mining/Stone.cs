using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public static int totalStore = 0;

    public delegate void StoneChoppedEvent();
    public static event StoneChoppedEvent OnStoreChopped;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Mine()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            //totalWood += maxHealth;

            InvokeChoppedEvent();

            StoneSpawner.Instance.RemoveStone(this);
            StoneSpawner.Instance.RespawnStone(this);

            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
    public void InvokeChoppedEvent()
    {
        OnStoreChopped?.Invoke();
    }
}
