using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WrathSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image icon;
    [SerializeField] Image chargeFill;
    [SerializeField] TMP_Text keyText;

    [Header("Colors")]
    [SerializeField] Color chargingColor = new(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] Color readyColor = Color.white;

    WrathManager manager;
    int slot;

    public void Init(WrathManager manager, int slot)
    {
        this.manager = manager;
        this.slot = slot;

        icon.sprite = manager.GetWrath(slot).Icon;
        keyText.text = manager.GetKeyName(slot);
    }

    void Update()
    {
        bool ready = manager.IsReady(slot);

        chargeFill.transform.localScale = new Vector3(manager.GetChargePercent(slot), 1f, 1f);
        icon.color = ready ? readyColor : chargingColor;
    }
}