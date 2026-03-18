using UnityEngine;
using System.Collections.Generic;

public static class DiceRoller
{
    public static DiceRollResult Roll(Dice dice)
    {
        // теперь используем метод Roll() самого куба
        int value = dice.Roll();
        return new DiceRollResult(dice, value);
    }

    public static List<DiceRollResult> Roll(List<Dice> diceList)
    {
        List<DiceRollResult> results = new();
        foreach (var dice in diceList)
            results.Add(Roll(dice));
        return results;
    }
}