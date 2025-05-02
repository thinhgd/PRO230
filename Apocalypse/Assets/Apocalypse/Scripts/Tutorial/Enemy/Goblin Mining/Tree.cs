using UnityEngine;
using TMPro;

public class Tree : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public static int totalWood = 0;

    public delegate void TreeChoppedEvent();
    public static event TreeChoppedEvent OnTreeChopped;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Chop()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            //totalWood += maxHealth;

            InvokeChoppedEvent();

            TreeSpawner.Instance.RemoveTree(this);
            TreeSpawner.Instance.RespawnTree(this);

            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
    public void InvokeChoppedEvent()
    {
        OnTreeChopped?.Invoke();
    }
}
