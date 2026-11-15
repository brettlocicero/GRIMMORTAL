using TMPro;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] int currency = 0;

    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyText;

    void Start()
    {
        UpdateCurrencyUI();
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        UpdateCurrencyUI();
    }

    void UpdateCurrencyUI()
    {
        currencyText.SetText("{0}", currency);
    }
}
