using System.Collections.Generic;

public static class DiceSelection
{
    public static List<Dice> Filter(
        List<Dice> allDice,
        List<string> selectedIds)
    {
        return allDice.FindAll(d => selectedIds.Contains(d.id));
    }
}