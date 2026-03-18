using UnityEngine;
using System.Text;

public static class BattleLogger
{
    private static StringBuilder log = new StringBuilder();

    public static void StartTurn()
    {
        log.Clear();
        Add("=== PLAYER ATTACK START ===");
    }

    public static void Add(string message)
    {
        log.AppendLine(message);
    }

    public static void EndTurn()
    {
        Add("=== ATTACK END ===");
        Debug.Log(log.ToString());
    }
}
