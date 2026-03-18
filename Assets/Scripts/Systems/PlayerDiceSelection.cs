using System.Collections.Generic;

public static class PlayerDiceSelection
{
    public static List<string> selectedDiceIds = new();

    public static void Clear()
    {
        selectedDiceIds.Clear();
    }
}