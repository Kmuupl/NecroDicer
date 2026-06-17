using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleAI
{
    private List<Dice> pool = new();
    private Armor playerArmor => PlayerCombat.Instance.armor;

    public void Init(EnemyData data)
    {
        pool.Clear();
        for (int i = 0; i < data.dicePoolSize; i++)
        {
            DiceType type = data.dicePool != null && data.dicePool.Length > 0
                ? data.dicePool[i % data.dicePool.Length]
                : DiceType.White;
            pool.Add(DiceFactory.Create(type, $"enemy_{i}"));
        }
    }

    public IEnumerator DoTurn(float delayBetweenDice, System.Action onDone)
    {
        if (playerArmor == null) { onDone?.Invoke(); yield break; }

        var shuffled = new List<Dice>(pool);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (var dice in shuffled)
        {
            int rolled = dice.Roll();
            Debug.Log($"[EnemyBattleAI] Враг бросает {dice.id} → {rolled}");

            bool placed = false;
            for (int i = 0; i < playerArmor.slots.Count; i++)
            {
                if (!playerArmor.slots[i].IsFilled && playerArmor.slots[i].TryFill(dice, rolled))
                {
                    Debug.Log($"[EnemyBattleAI] {dice.id} ({rolled}) → слот игрока {i}");
                    placed = true;
                    break;
                }
            }

            if (!placed)
                Debug.Log($"[EnemyBattleAI] {dice.id} ({rolled}) не подошёл ни к одному слоту");

            yield return new WaitForSeconds(delayBetweenDice);

            if (playerArmor.IsFullyPierced()) break;
        }

        onDone?.Invoke();
    }
}