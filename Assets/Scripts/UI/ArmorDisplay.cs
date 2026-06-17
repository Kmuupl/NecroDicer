using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmorDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] conditionText;
    private ArmorSlot[] slots;
    private Armor armor;
    public void Init(Armor armorData)
    {
        armor = armorData;
        slots = armor.slots.ToArray();

        for (int i = 0; i < conditionText.Length; i++)
        {
            if (i < slots.Length)
            {
                UpdateSlotText(i);
                SetupSlotButton(i);
            }
        }
    }

    private void SetupSlotButton(int index)
    {
        Button btn = conditionText[index].GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogWarning($"Нет кнопки для слота {index} в ArmorDisplay!");
            return;
        }

        btn.onClick.RemoveAllListeners();
        int capturedIndex = index; // Захватываем индекс для замыкания
        btn.onClick.AddListener(() => OnSlotClicked(capturedIndex));
    }

    private void OnSlotClicked(int slotIndex)
    {
        var bm = BattleManager.Instance;
        if (bm.SelectedDice == null)
        {
            Debug.Log("Нет выбранного куба.");
            return;
        }

        var dice = bm.SelectedDice; // сохраняем до ClearSelection
        int value = bm.SelectedDiceValue;

        bool success = armor.TryFillSlot(slotIndex, dice, value);
        if (success)
        {
            BattlefieldPanel.Instance.RemoveDice(dice);
            OnDicePlaced(slotIndex);
            bm.ClearSelection();
            Debug.Log($"Куб {dice.id} успешно размещён в слоте {slotIndex}!");
        }
        else
        {
            Debug.Log($"Куб не подходит для слота {slotIndex}.");
        }
    }

    private void UpdateSlotText(int index)
    {
        var condition = slots[index].condition;
        conditionText[index].text = condition.mustBeGreater
            ? $">{condition.requiredValue}"
            : $"{condition.requiredValue}";
    }

    public void OnDicePlaced(int slotIndex)
    {
        if (slotIndex < conditionText.Length)
        {
            conditionText[slotIndex].gameObject.SetActive(false);
        }
    }

    public void OnSlotCleared(int slotIndex)
    {
        if (slotIndex < conditionText.Length)
        {
            conditionText[slotIndex].gameObject.SetActive(true);
            UpdateSlotText(slotIndex);
        }
    }
}