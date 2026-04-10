using System.Collections.Generic;
using UnityEngine;

public class ArmorPanel : MonoBehaviour
{
    [SerializeField] private ArmorSlotView slotPrefab;
    [SerializeField] private Transform slotsContainer;
    private Armor currentArmor;
    private List<ArmorSlotView> spawnedSlots = new();
    public void Init(Armor armor)
    {
        // чистим слоты в любом случае
        foreach (var slot in spawnedSlots)
            Destroy(slot.gameObject);
        spawnedSlots.Clear();

        currentArmor = armor;

        // если броня null — просто очищаем панель
        if (armor == null) return;

        SpawnSlots();
    }
    private void SpawnSlots()
    {
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
        for (int i = 0; i < currentArmor.slots.Count; i++)
        {
            ArmorSlotView view = Instantiate(slotPrefab, slotsContainer);
            view.Init(currentArmor.slots[i], i);
            spawnedSlots.Add(view);
        }
    }
    public ArmorSlotView GetSlotView(int index)
    {
        if (index < 0 || index >= spawnedSlots.Count) return null;
        return spawnedSlots[index];
    }

}