using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWoodManager : MonoBehaviour
{
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI goldText;
    public Button summonButton;
    public int woodRequiredToSummon = 10;

    void Awake()
    {
        TreeItem.totalWood = 0;
        Stone.totalStore = 0;
        summonButton.interactable = false;
    }

    void OnEnable()
    {
        TreeItem.OnTreeChopped += UpdateWoodUI;
        Stone.OnStoreChopped += UpdateWoodUI;
    }

    void OnDisable()
    {
        TreeItem.OnTreeChopped -= UpdateWoodUI;
        Stone.OnStoreChopped -= UpdateWoodUI;
    }

    void UpdateWoodUI()
    {
        CastleManager castle = FindObjectOfType<CastleManager>();
        if (castle != null)
        {
            woodText.text = "Gỗ: " + castle.wood;
            goldText.text = "Gold: " + castle.stone;
        }
        summonButton.interactable = TreeItem.totalWood >= woodRequiredToSummon;
    }

    public void OnClickSummonWorker()
    {
        if (TreeItem.totalWood >= woodRequiredToSummon)
        {
            TreeItem.totalWood -= woodRequiredToSummon;
            summonButton.interactable = false;
            woodText.text = "Gỗ: " + TreeItem.totalWood;
            FindObjectOfType<WorkerSpawner>().SpawnWorker();
        }
    }
}
