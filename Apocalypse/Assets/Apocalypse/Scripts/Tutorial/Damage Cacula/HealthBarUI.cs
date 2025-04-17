using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    private float maxHP = 100f;
    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    public void SetHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateUI();
    }

    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();
    }

    private void UpdateUI()
    {
        hpSlider.value = currentHP / maxHP;
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP:F1} / {maxHP}";
        }
    }

    public float GetCurrentHP() => currentHP;
}
