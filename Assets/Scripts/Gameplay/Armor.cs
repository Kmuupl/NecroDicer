using System.Collections.Generic;
using UnityEngine;

//условие слота брони
public class ArmorCondition
{
    public int requiredValue; //нужное значение
    public bool mustBeGreater; //   больше или равно нужного значения

    //проверка куба на условие слота брони
    public bool Check(int diceValue)
    {
        if (mustBeGreater)
            return diceValue > requiredValue;
        else
            return diceValue == requiredValue;
    }
}

//один слот брони
public class ArmorSlot
{
    public ArmorCondition condition; //что требует слот
    public Dice filledBy = null; //какой куб вставлен

    //слот заполнен, если есть куб
    public bool IsFilled => filledBy != null;

    //попытка вставить куб
    //rolledValue - значение, которое выпало на кубе
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

    //очистить слот (если нужно будет сбросить броню)
    public void Clear()
    {
        filledBy = null;
    }
}

//броня - набор слотов
public class Armor
{
    public List<ArmorSlot> slots = new();

    //слоты заполнены - броян пробита
    public bool IsFullyPierced()
    {
        foreach (var slot in slots)
            if (!slot.IsFilled) return false;
        return true;
    }

    //попытка вставить куб в конкретный слот по индексу
    public bool TryFillSlot(int slotIndex, Dice dice, int rolledValue)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogWarning($"Неверный индекс слота: {slotIndex}");
            return false;
        }
        return slots[slotIndex].TryFill(dice, rolledValue);
    }

    //очистить все слоты
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

    //урон по заполненным слотам
    public float[] damagePercents;

    public void InitDefaultCurve()
    {
        damagePercents = new float[slots.Count + 1];
        for (int i = 0; i <= slots.Count; i++)
        {
            damagePercents[i] = i / (float)slots.Count; //линейная кривая от 0% до 100%
        }
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