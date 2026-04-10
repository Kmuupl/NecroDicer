using System.Collections.Generic;
using UnityEngine;

public class DiceBag
{
    //контейнеры для кубов(этапы продвижения кубов):
    public List<Dice> bag = new(); //мешок
    public List<Dice> pool = new(); //пул-кк
    public List<Dice> tray = new(); //трей
    public List<Dice> buttleField = new(); //поле битвы
    public List<Dice> discard = new(); //сброс
    public Dictionary<Dice, int> battleFieldRolls = new(); //хранит результаты бросков кубов на поле боя

    // сколько кубов выдается в трей за раунд(за раунд?):
    public int traySize = 12;

    //мешок:
    //добавить куб в мешок(как, решить потом):
    public void AddToBag(Dice dice)
    {
        bag.Add(dice);
    }

    //пул--кк:
    //добавить куб с мешка в пул(выбор кубов):
    public bool AddToPool(Dice dice)
    {
        if (!bag.Contains(dice))
        {
            Debug.LogWarning($"Куб {dice.id} не найден в мешке :()");
            return false;
        }
        bag.Remove(dice);
        pool.Add(dice);
        return true;
    }

    //убрать куб из пула в мешок(отмена выбора):
    public bool RemoveFromPool(Dice dice)
    {
        if (!pool.Contains(dice)) return false;
        pool.Remove(dice);
        bag.Add(dice);
        return true;
    }

    //трей:
    //случайные кубы из пула-кк в трей:
    public void DrawToTray()
    {
        int toDraw = 5;
        for (int i = 0; i < toDraw; i++)
        {
            if (pool.Count == 0)
            {
                if (discard.Count == 0) break; // нечего перемешивать
                ReshuffleDiscard();
                Debug.Log("Пул пуст — сброс перемешан обратно в пул.");
            }

            int randomIndex = Random.Range(0, pool.Count);
            Dice drawn = pool[randomIndex];
            pool.Remove(drawn);
            tray.Add(drawn);
        }
    }

    //поле боя:
    //выбрать куб из трей в поле боя:
    public bool ThrowToButtleField(Dice dice)
    {
        if (!tray.Contains(dice))
        {
            //не должно случиться, так как UI должен блокировать эти действия, но на всякий случай:
            Debug.LogWarning($"Куб {dice.id} не найден в трее");
            return false;
        }

        int rolledValue = dice.Roll();
        tray.Remove(dice);
        buttleField.Add(dice);
        battleFieldRolls[dice] = rolledValue;

        Debug.Log($"Куб {dice.id} брошен на поле боя с результатом {rolledValue}");
        return true;
    }

    //кубы с поля боя в сброс:
    public void EndTurn()
    {
        discard.AddRange(buttleField);
        buttleField.Clear();
        battleFieldRolls.Clear();

        discard.AddRange(tray);
        tray.Clear();

        Debug.Log($"[EndTurn] кубы с поля боя и трея перемещены в сброс. Сброс теперь содержит {discard.Count} кубов.");
    }

    //со сброса в мешок:
    public void ReshuffleDiscard()
    {
        pool.AddRange(discard);
        discard.Clear();
    }
}