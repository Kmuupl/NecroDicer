using TMPro;
using UnityEngine;

public class ArmorSlotView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI conditionText;
    private ArmorSlot slot;
    public void Init(ArmorSlot armorSlot)
    {
        slot = armorSlot;
        UpdateConditionText();
    }
    private void UpdateConditionText()
    {
        if (slot.condition.mustBeGreater)
            conditionText.text = $">{slot.condition.requiredValue}";
        else
            conditionText.text = $"={slot.condition.requiredValue}";
    }
    public void OnDicePlaced()
    {
        conditionText.gameObject.SetActive(false);
    }
    public void OnSlotCleared()
    {
        conditionText.gameObject.SetActive(true);
        UpdateConditionText();
    }
}