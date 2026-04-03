using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Lumin;

public class CombatSystem : MonoBehaviour
{
    public GameObject battlePanel; // панель с UI боя
    private Dice selectedDice = null;
    // временный урон оружия (потом заменим на реальное оружие)
    public Weapon weapon;
    // враг которого атакуем в этом раунде
    // null = бой ещё не начался или враг не выбран
    private Enemy targetEnemy = null;
    // ссылка на мешок
    private DiceBag diceBag => PlayerDiceManager.Instance.diceBag;
    public HPBar enemyHPBar;

    void Update()
    {
        // если бой не начался — ничего не делаем
        if (targetEnemy == null) return;

        // Q/W/E/R/T — бросить куб из трея по индексу
        if (Input.GetKeyDown(KeyCode.Q)) ThrowDiceFromTray(0);
        if (Input.GetKeyDown(KeyCode.W)) ThrowDiceFromTray(1);
        if (Input.GetKeyDown(KeyCode.E)) ThrowDiceFromTray(2);
        if (Input.GetKeyDown(KeyCode.R)) ThrowDiceFromTray(3);
        if (Input.GetKeyDown(KeyCode.T)) ThrowDiceFromTray(4);

        // 1/2/3 — вставить куб с поля боя в слот брони врага
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryFillArmorSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryFillArmorSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryFillArmorSlot(2);

        // Space — атаковать (конец раунда)
//        if (Input.GetKeyDown(KeyCode.Space))
//            ResolveAttack();

        // Z/X/C/V/B — выбрать куб с поля боя по индексу
        if (Input.GetKeyDown(KeyCode.Z)) SelectDiceFromBattlefield(0);
        if (Input.GetKeyDown(KeyCode.X)) SelectDiceFromBattlefield(1);
        if (Input.GetKeyDown(KeyCode.C)) SelectDiceFromBattlefield(2);
        if (Input.GetKeyDown(KeyCode.V)) SelectDiceFromBattlefield(3);
        if (Input.GetKeyDown(KeyCode.B)) SelectDiceFromBattlefield(4);

        // H — конец хода игрока → ход врага
//        if (Input.GetKeyDown(KeyCode.H))
//            PlayerEndTurn();
    }

    // вызывается когда игрок нажимает на врага
    public void StartBattle(Enemy enemy)
    {

        if (targetEnemy != null)
        {
            Debug.LogWarning("Бой уже начался! Нельзя начать новый бой, пока не закончится текущий.");
            return;
        }
        targetEnemy = enemy;
        battlePanel?.SetActive(true);
        enemy.hpBar = enemyHPBar;
        enemyHPBar?.UpdateBar(enemy.currentHP, enemy.maxHP);
        Debug.Log($"Бой начался! Враг: {enemy.name}");
        StartTurn();
    }

    // начало хода
    void StartTurn()
    {
        // для теста — кубы из мешка в пул
        foreach (var dice in diceBag.bag.ToArray())
            diceBag.AddToPool(dice);

        diceBag.DrawToTray();

        BattleLogger.StartTurn();
        BattleLogger.Add($"Начало хода. В трей добавлено {diceBag.tray.Count} кубов.");

        foreach (var dice in diceBag.tray)
            BattleLogger.Add($"- {dice.id}");

        // показываем броню врага
        BattleLogger.Add("ARMOR:");
        for (int i = 0; i < targetEnemy.armor.slots.Count; i++)
        {
            ArmorSlot slot = targetEnemy.armor.slots[i];
            string condition = slot.condition.mustBeGreater
                ? $"> {slot.condition.requiredValue}"
                : $"= {slot.condition.requiredValue}";
            string status = slot.IsFilled ? $"заполнен: {slot.filledBy.id}" : "пустой";
            BattleLogger.Add($"- Слот {i}: {condition}, {status}");
        }
    }

    // бросить куб из трея по индексу на поле боя
    void ThrowDiceFromTray(int trayIndex)
    {
        if (diceBag.tray.Count == 0)
        {
            Debug.Log("Трей пуст.");
            return;
        }

        if (trayIndex >= diceBag.tray.Count)
        {
            Debug.Log($"Куба с индексом {trayIndex} нет в трее. Кубов в трее: {diceBag.tray.Count}");
            return;
        }

        Dice dice = diceBag.tray[trayIndex];
        diceBag.ThrowToButtleField(dice);

        int rolledValue = diceBag.battleFieldRolls[dice];
        BattleLogger.Add($"Брошен куб {dice.id} с результатом {rolledValue}.");
    }

    // вставить куб с поля боя в слот брони врага по индексу
    void TryFillArmorSlot(int slotIndex)
    {
        // проверяем что куб выбран
        if (selectedDice == null)
        {
            Debug.Log("Сначала выбери куб с поля боя (Z/X/C/V/B).");
            return;
        }

        // проверяем что выбранный куб всё ещё на поле боя
        if (!diceBag.buttleField.Contains(selectedDice))
        {
            Debug.Log("Выбранный куб уже не на поле боя.");
            selectedDice = null;
            return;
        }

        int rolledValue = diceBag.battleFieldRolls[selectedDice];
        bool filled = targetEnemy.armor.TryFillSlot(slotIndex, selectedDice, rolledValue);

        if (filled)
        {
            diceBag.buttleField.Remove(selectedDice);
            diceBag.battleFieldRolls.Remove(selectedDice);
            Debug.Log($"Куб {selectedDice.id} со значением {rolledValue} вставлен в слот {slotIndex} брони врага.");
            BattleLogger.Add($"Куб {selectedDice.id} ({rolledValue}) вставлен в слот {slotIndex}.");
            BattleLogger.Add($"Заполнено слотов: {targetEnemy.armor.FilledSlotsCount()}/{targetEnemy.armor.slots.Count}");
            selectedDice = null; // сбрасываем выбор после вставки
        }
    }

    // атаковать — наносим урон, кубы с поля и брони в сброс
    // НО ход не заканчивается — игрок может атаковать снова
    // В CombatSystem.cs — найди void ResolveAttack() и замени сигнатуру:
    public int ResolveAttack()
    {
        if (targetEnemy == null || targetEnemy.gameObject == null) return 0;

        int weaponRoll = weapon != null ? weapon.RollDamage() : 5;
        int damage = targetEnemy.armor.CalcDamage(weaponRoll);
        BattleLogger.Add($"Атака! Урон оружия: {weaponRoll}. Заполнено {targetEnemy.armor.FilledSlotsCount()} из {targetEnemy.armor.slots.Count} слотов.");

        if (damage > 0)
        {
            targetEnemy.TakeDamage(damage);
        }
        else
        {
            BattleLogger.Add("Урон не нанесён — ни один слот не заполнен.");
            damage = 0;
        }

        if (targetEnemy != null && targetEnemy.currentHP > 0)
            targetEnemy.armor.ClearAll();

        selectedDice = null;
        diceBag.buttleField.Clear();
        diceBag.battleFieldRolls.Clear();

        BattleLogger.Add("Можно атаковать снова или нажать H для конца хода.");
        BattleLogger.EndTurn();

        return damage;
    }

    // конец хода — кубы с поля боя в сброс
    void EndTurn()
    {
        selectedDice = null;
        diceBag.EndTurn();

        if (targetEnemy == null || targetEnemy.gameObject == null || targetEnemy.currentHP <= 0)
        {
            Debug.Log("Бой окончен!");
            targetEnemy = null;
            return;
        }

        // ход врага
        Debug.Log("Ход врага...");
        targetEnemy.Attack();

        if (PlayerHealth.Instance == null || PlayerHealth.Instance.currentHP <= 0)
        {
            battlePanel?.SetActive(false);
            Debug.Log("Игрок погиб! Бой окончен.");
            targetEnemy = null;
            return;
        }

        StartTurn();
    }

    // выбрать куб с поля боя по индексу
    void SelectDiceFromBattlefield(int index)
    {
        if (index >= diceBag.buttleField.Count)
        {
            Debug.Log($"Куба с индексом {index} нет на поле боя.");
            return;
        }

        selectedDice = diceBag.buttleField[index];
        int rolledValue = diceBag.battleFieldRolls[selectedDice];
        Debug.Log($"Выбран куб {selectedDice.id} со значением {rolledValue}.");
    }

    // конец хода игрока — передаём ход врагу
    void PlayerEndTurn()
    {
        BattleLogger.Add("Конец хода игрока.");
        BattleLogger.EndTurn();
        EndTurn();
    }
    public void ClearBattlefield()
    {
        selectedDice = null;
        diceBag.buttleField.Clear();
        diceBag.battleFieldRolls.Clear();
    }
}