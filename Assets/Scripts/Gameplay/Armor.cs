using System.Collections.Generic;
using UnityEngine;

public class ArmorCondition
{
    public int requiredValue;
    public bool mustBeGreater;

    public bool Check(int diceValue)
    {
        if (mustBeGreater)
            return diceValue > requiredValue;
        else
            return diceValue == requiredValue;
    }
}

public class ArmorSlot
{
    public ArmorCondition condition;
    public Dice filledBy = null;

    public bool IsFilled => filledBy != null;

    public bool TryFill(Dice dice, int rolledValue)
    {
        if (IsFilled)
        {
            Debug.Log("Слот уже заполнен.");
            return false;
        }

        if (!condition.Check(rolledValue))
        {
            Debug.Log($"Куб {dice.id} с результатом {rolledValue} не подходит для этого слота.");
            return false;
        }

        filledBy = dice;
        return true;
    }

    public void Clear()
    {
        filledBy = null;
    }
}

public class Armor
{
    public List<ArmorSlot> slots = new();
    public float[] damagePercents;

    // пустой конструктор — для совместимости
    public Armor() { }

    // конструктор из данных — вот он ВНУТРИ класса
    public Armor(ArmorData data)
    {
        slots = new List<ArmorSlot>();

        foreach (var condData in data.conditions)
        {
            slots.Add(new ArmorSlot
            {
                condition = new ArmorCondition
                {
                    requiredValue = condData.requiredValue,
                    mustBeGreater = condData.mustBeGreater
                }
            });
        }

        damagePercents = data.damagePercents;
    }

    public bool IsFullyPierced()
    {
        foreach (var slot in slots)
            if (!slot.IsFilled) return false;
        return true;
    }

    public bool TryFillSlot(int slotIndex, Dice dice, int rolledValue)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogWarning($"Неверный индекс слота: {slotIndex}");
            return false;
        }
        return slots[slotIndex].TryFill(dice, rolledValue);
    }

    public void ClearAll()
    {
        foreach (var slot in slots)
            slot.Clear();
    }

    public int FilledSlotsCount()
    {
        int count = 0;
        foreach (var slot in slots)
            if (slot.IsFilled) count++;
        return count;
    }

    public void InitDefaultCurve()
    {
        damagePercents = new float[slots.Count + 1];
        for (int i = 0; i <= slots.Count; i++)
            damagePercents[i] = i / (float)slots.Count;
    }

    public int CalcDamage(int weaponDamage)
    {
        int filled = FilledSlotsCount();
        if (damagePercents == null || damagePercents.Length == 0)
            InitDefaultCurve();
        float percent = damagePercents[filled];
        return Mathf.RoundToInt(weaponDamage * percent);
    }
}