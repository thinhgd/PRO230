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
        Tree.totalWood = 0;
        Stone.totalStore = 0;
        summonButton.interactable = false;
    }

    void OnEnable()
    {
        Tree.OnTreeChopped += UpdateWoodUI;
        Stone.OnStoreChopped += UpdateWoodUI;
    }

    void OnDisable()
    {
        Tree.OnTreeChopped -= UpdateWoodUI;
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
        summonButton.interactable = Tree.totalWood >= woodRequiredToSummon;
    }

    public void OnClickSummonWorker()
    {
        if (Tree.totalWood >= woodRequiredToSummon)
        {
            Tree.totalWood -= woodRequiredToSummon;
            summonButton.interactable = false;
            woodText.text = "Gỗ: " + Tree.totalWood;
            FindObjectOfType<WorkerSpawner>().SpawnWorker();
        }
    }
}
