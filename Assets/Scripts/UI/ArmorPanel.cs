using Unity.Burst.Intrinsics;
using UnityEngine;

public class ArmorPanel : MonoBehaviour
{
    [SerializeField] private ArmorDisplay armorPanel_1;
    [SerializeField] private ArmorDisplay armorPanel_2;
    [SerializeField] private ArmorDisplay armorPanel_3;
    [SerializeField] private ArmorDisplay armorPanel_4;

    private ArmorDisplay currentDisplay;
    private Armor currentArmor;

    public void Init(Armor armor)
    {
        if (currentDisplay != null)
        {
            Destroy(currentDisplay.gameObject);
        }
        if (armor == null) return;
        currentArmor = armor;
        ArmorDisplay displayPrefab = GetPrefabForSlotCount(armor.slots.Count);
        if (displayPrefab == null)
        {
            Debug.LogError($"Нет подходящего префаба для брони с {armor.slots.Count} слотами!");
            return;
        }
        currentDisplay = Instantiate(displayPrefab, transform);
        currentDisplay.Init(armor);
    }

    private ArmorDisplay GetPrefabForSlotCount(int count)
    {
        return count switch
        {
            1 => armorPanel_1,
            2 => armorPanel_2,
            3 => armorPanel_3,
            4 => armorPanel_4,
            _ => null
        };
    }

    public void IfDicePlaced(int slotIndex)
    {
        currentDisplay?.OnDicePlaced(slotIndex);
    }

    public void IfSlotCleared(int slotIndex)
    {
        currentDisplay?.OnSlotCleared(slotIndex);
    }
}