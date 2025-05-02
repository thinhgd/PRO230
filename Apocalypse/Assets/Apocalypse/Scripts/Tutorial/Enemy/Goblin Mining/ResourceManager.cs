using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int woodAmount = 0;
    public TextMeshProUGUI woodText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateWoodUI();
    }

    public void AddWood(int amount)
    {
        woodAmount += amount;
        UpdateWoodUI();
    }

    private void UpdateWoodUI()
    {
        woodText.text = "Wood: " + woodAmount;
    }
}
