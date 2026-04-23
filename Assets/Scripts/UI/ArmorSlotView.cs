using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmorSlotView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI conditionText;
    private ArmorSlot slot;
    private int slotIndex;
    public void Init(ArmorSlot armorSlot, int index)
    {
        slot = armorSlot;
        slotIndex = index;
        UpdateConditionText();
        GetComponent<Button>().onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        var bm = BattleManager.Instance;
        if (bm.SelectedDice == null)
        {
            Debug.Log("Select Dice :)");
            return;
        }
        bool success = BattleManager.Instance.CurrentEnemy.armor.TryFillSlot(slotIndex, bm.SelectedDice, bm.SelectedDiceValue);
        if (success)
        {
            Debug.Log($"Куб {bm.SelectedDice.id} со значением {bm.SelectedDiceValue} помещён в слот {slotIndex}");
            OnDicePlaced();
            bm.ClearSelection();
        }
        else
        {
            Debug.Log($"Куб {bm.SelectedDice.id} со значением {bm.SelectedDiceValue} не подходит для слота {slotIndex}");
        }
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